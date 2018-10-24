using ESFA.DC.DateTimeProvider.Interface;

namespace ESFA.DC.EAS1819.Service.Test.Import
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using ESFA.DC.EAS1819.DataService;
    using ESFA.DC.EAS1819.DataService.Interface;
    using ESFA.DC.EAS1819.DataService.Interface.FCS;
    using ESFA.DC.EAS1819.EF;
    using ESFA.DC.EAS1819.Interface.Reports;
    using ESFA.DC.EAS1819.Service.Import;
    using ESFA.DC.EAS1819.Tests.Base.Builders;
    using ESFA.DC.Logging;
    using ESFA.DC.Logging.Config;
    using Moq;
    using Xunit;
    using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

    public class ImportServiceShould
    {
        private readonly EasValidationService _validationService;
        private readonly EasSubmissionService _easSubmissionService;
        private readonly IRepository<SourceFile> _sourceFileRepo;
        private readonly IRepository<ValidationError> _validationErrorRepo;
        private readonly ValidationErrorService _validationErrorService;
        private readonly Mock<IReportingController> reportingController;
        private readonly Mock<IFundingLineContractTypeMappingDataService> fundingLineContractTypeMock;
        private readonly Mock<IFCSDataService> fcsDataServiceMock;
        private readonly Mock<IEasPaymentService> _easPaymentServiceMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

        public ImportServiceShould()
        {
            var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];
            IRepository<EasSubmission> easSubmissionRepository = new Repository<EasSubmission>(context: new EasdbContext(connString));
            IRepository<EasSubmissionValues> easSubmissionValuesRepository = new Repository<EasSubmissionValues>(context: new EasdbContext(connString));
            _validationErrorRepo = new Repository<ValidationError>(context: new EasdbContext(connString));
            _sourceFileRepo = new Repository<SourceFile>(context: new EasdbContext(connString));
            _easSubmissionService = new EasSubmissionService(easSubmissionRepository, easSubmissionValuesRepository, new EasdbContext(connString), new SeriLogger(new ApplicationLoggerSettings(), new ExecutionContext(), null));
            _validationErrorService = new ValidationErrorService(_validationErrorRepo, _sourceFileRepo);

            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            reportingController = new Mock<IReportingController>();
            _easPaymentServiceMock = new Mock<IEasPaymentService>();
            fcsDataServiceMock = new Mock<IFCSDataService>();
            fundingLineContractTypeMock = new Mock<IFundingLineContractTypeMappingDataService>();

            fcsDataServiceMock.Setup(x => x.GetContractsForProvider(It.IsAny<int>())).Returns(new ContractAllocationsBuilder().Build);
            fundingLineContractTypeMock.Setup(x => x.GetAllFundingLineContractTypeMappings()).Returns(new FundingLineContractTypeMappingsBuilder().Build);
            _easPaymentServiceMock.Setup(x => x.GetAllPaymentTypes()).Returns(new PaymentTypesBuilder().GetPaymentTypeList);
            _dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(DateTime.UtcNow);
            _validationService = new EasValidationService(_easPaymentServiceMock.Object, _dateTimeProviderMock.Object, _validationErrorService, fcsDataServiceMock.Object, fundingLineContractTypeMock.Object);
        }

        [Fact]
        public void ImportValidEasRecords()
        {
            var fileInfo = new EasFileInfoBuilder().Build();
            var submissionId = Guid.NewGuid();
            var sut = new ImportService(
                                                            submissionId,
                                                            _easSubmissionService,
                                                            _easPaymentServiceMock.Object,
                                                            _validationService,
                                                            reportingController.Object,
                new SeriLogger(new ApplicationLoggerSettings(), new Logging.ExecutionContext(), null));

            sut.ImportEasDataAsync(fileInfo, new EasCsvRecordBuilder().GetValidRecords(), CancellationToken.None).GetAwaiter().GetResult();
            var easSubmissionValues = _easSubmissionService.GetEasSubmissionValues(submissionId);
            var sourceFile = _sourceFileRepo.TableNoTracking.Where(x => x.FileName == fileInfo.FileName)
                .OrderByDescending(x => x.SourceFileId).FirstOrDefault();

            var validationErrors = _validationErrorRepo.TableNoTracking.Where(x => x.SourceFileId == sourceFile.SourceFileId).ToList();
            Assert.NotEmpty(easSubmissionValues);
            Assert.Equal(2, easSubmissionValues.Count);
            Assert.Empty(validationErrors);
        }

        [Fact]
        public void ImportValidRecordsAndLogValidationErrorsForInvalidRecords()
        {
            var fileInfo = new EasFileInfoBuilder().WithFileName("EASDATA-10033670-20180909-090909.csv").WithFilePath(@"SampleEASFiles\Mixed\EASDATA-10033670-20180909-090909.csv").Build();
            var validAndInvalidRecords = new EasCsvRecordBuilder().GetValidAndInvalidRecords();
            var submissionId = Guid.NewGuid();
            var sut = new ImportService(
                submissionId,
                _easSubmissionService,
                _easPaymentServiceMock.Object,
                _validationService,
                reportingController.Object,
                new SeriLogger(new ApplicationLoggerSettings(), new Logging.ExecutionContext(), null));

            sut.ImportEasDataAsync(fileInfo, validAndInvalidRecords, CancellationToken.None).GetAwaiter().GetResult();
            var easSubmissionValues = _easSubmissionService.GetEasSubmissionValues(submissionId);
            var sourceFile = _sourceFileRepo.TableNoTracking.Where(x => x.FileName == fileInfo.FileName)
                .OrderByDescending(x => x.SourceFileId).FirstOrDefault();
            var validationErrors = _validationErrorRepo.TableNoTracking.Where(x => x.SourceFileId == sourceFile.SourceFileId).ToList();

            Assert.NotEmpty(easSubmissionValues);
            Assert.Equal(2, easSubmissionValues.Count);
            Assert.NotEmpty(validationErrors);
            Assert.Equal(5, validationErrors.Count);
        }
    }
}
