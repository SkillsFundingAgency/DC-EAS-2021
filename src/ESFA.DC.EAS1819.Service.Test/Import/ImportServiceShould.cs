//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using ESFA.DC.EAS1819.DataService.Interface.FCS;
//using ESFA.DC.EAS1819.Interface.Reports;
//using ESFA.DC.EAS1819.Model;
//using ESFA.DC.EAS1819.ReportingService;
//using ESFA.DC.EAS1819.Service.Mapper;
//using ESFA.DC.EAS1819.Service.Providers;
//using ESFA.DC.IO.AzureStorage;
//using ESFA.DC.Logging;
//using ESFA.DC.Logging.Config;
//using ESFA.DC.ReferenceData.FCS.Model;
//using Moq;
//using Serilog;
//using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

//namespace ESFA.DC.EAS1819.Service.Test.Import
//{
//    using System;
//    using System.Configuration;
//    using System.IO;
//    using ESFA.DC.EAS1819.DataService;
//    using ESFA.DC.EAS1819.DataService;
//    using ESFA.DC.EAS1819.DataService.Interface;
//    using ESFA.DC.EAS1819.EF;
//    using ESFA.DC.EAS1819.Service.Import;
//    using Xunit;

//    public class ImportServiceShould
//    {
//        EasValidationService _validationService;
//        EasSubmissionService _easSubmissionService;
//        EasPaymentService _easPaymentService;
//        CsvParser _csvParser;
//        Mock<IReportingController> reportingController;
//        Mock<IFundingLineContractTypeMappingDataService> fundingLineContractTypeMock;
//        Mock<IFCSDataService> fcsDataServiceMock;

//        public ImportServiceShould()
//        {
//            var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];
//            _csvParser = new CsvParser();
//            IRepository<PaymentTypes> paymentRepository = new Repository<PaymentTypes>(context: new EasdbContext(connString));
//            _easPaymentService = new EasPaymentService(paymentRepository);
//            IRepository<EasSubmission> easSubmissionRepository = new Repository<EasSubmission>(context: new EasdbContext(connString));
//            IRepository<EasSubmissionValues> easSubmissionValuesRepository = new Repository<EasSubmissionValues>(context: new EasdbContext(connString));
//            IRepository<ValidationError> validationErrorRepo = new Repository<ValidationError>(context: new EasdbContext(connString));
//            IRepository<SourceFile> sourceFileRepo = new Repository<SourceFile>(context: new EasdbContext(connString));
//            _easSubmissionService = new EasSubmissionService(easSubmissionRepository, easSubmissionValuesRepository, new EasdbContext(connString), new SeriLogger(new ApplicationLoggerSettings(), new ExecutionContext(), null));
//            ValidationErrorService validationErrorService = new ValidationErrorService(validationErrorRepo, sourceFileRepo);
//            fcsDataServiceMock = new Mock<IFCSDataService>();

//            fcsDataServiceMock.Setup(x => x.GetContractsForProvider(It.IsAny<int>())).Returns(BuildContractAllocations);
//            fundingLineContractTypeMock = new Mock<IFundingLineContractTypeMappingDataService>();
//            fundingLineContractTypeMock.Setup(x => x.GetAllFundingLineContractTypeMappings()).Returns(BuildFundingLineContractMappings());

//            _validationService = new EasValidationService(_easPaymentService, new DateTimeProvider.DateTimeProvider(), _csvParser, validationErrorService, fcsDataServiceMock.Object, fundingLineContractTypeMock.Object);
//            reportingController = new Mock<IReportingController>();
//        }

//        [Fact]
//        public void ImportEasCsv()
//        {
//            var fileInfo = new EasFileInfo()
//            {
//                FileName = "EAS-10033670-1819-20180912-144437-03.csv",
//                UKPRN = "10033670",
//                DateTime = DateTime.UtcNow,
//                FilePreparationDate = DateTime.UtcNow.AddHours(-2),
//                FilePath = @"SampleEASFiles\Valid\EAS-10033670-1819-20180912-144437-03.csv"
//            };
//            var easFileDataProviderService = new EASFileDataProviderService();
//            var submissionId = Guid.NewGuid();
//            ImportService importService = new ImportService(
//                                                            submissionId,
//                                                            _easSubmissionService,
//                                                            _easPaymentService,
//                                                            easFileDataProviderService,
//                                                            new CsvParser(),
//                                                            _validationService,
//                                                            reportingController.Object,
//                                                            new AzureStorageKeyValuePersistenceService(null),
//                new SeriLogger(new ApplicationLoggerSettings(), new Logging.ExecutionContext(), null));
//            importService.ImportEasDataAsync(fileInfo, CancellationToken.None).GetAwaiter().GetResult();
//            var easSubmissionValues = _easSubmissionService.GetEasSubmissionValues(submissionId);
//            Assert.NotEmpty(easSubmissionValues);
//            Assert.Equal(2, easSubmissionValues.Count);
//        }

//        [Fact]
//        public void ImportValidRecordsAndLogValidationErrorsForInvalidRecords()
//        {
//            var fileInfo = new EasFileInfo()
//            {
//                FileName = "EASDATA-10033670-20180909-090909.csv",
//                UKPRN = "10033670",
//                DateTime = DateTime.UtcNow,
//                FilePreparationDate = DateTime.UtcNow.AddHours(-2),
//                FilePath = @"SampleEASFiles\Mixed\EASDATA-10033670-20180909-090909.csv"
//            };

//            var easFileDataProviderService = new EASFileDataProviderService();
//            var submissionId = Guid.NewGuid();
//            ImportService importService = new ImportService(
//                                                            submissionId,
//                                                            _easSubmissionService,
//                                                            _easPaymentService,
//                                                            easFileDataProviderService,
//                                                            new CsvParser(),
//                                                            _validationService,
//                                                            reportingController.Object,
//                                                            new AzureStorageKeyValuePersistenceService(null),
//                                                            new SeriLogger(new ApplicationLoggerSettings(), new Logging.ExecutionContext(), null));
//            importService.ImportEasDataAsync(fileInfo, CancellationToken.None).GetAwaiter().GetResult();
//            var easSubmissionValues = _easSubmissionService.GetEasSubmissionValues(submissionId);
//            Assert.NotEmpty(easSubmissionValues);
//            Assert.Equal(2, easSubmissionValues.Count);
//        }

//        [Fact]
//        public void ValidateEasRecords()
//        {
//            var fileInfo = new EasFileInfo()
//            {
//                FileName = "EAS-10033670-1819-20180812-100221-05.csv",
//                UKPRN = "10033670",
//                DateTime = DateTime.UtcNow,
//                FilePreparationDate = DateTime.UtcNow.AddHours(-2),
//                FilePath = @"SampleEASFiles\Invalid\EAS-10033670-1819-20180812-100221-05.csv"
//            };

//            var easFileDataProviderService = new EASFileDataProviderService();
//            var streamReader = easFileDataProviderService.Provide(fileInfo, CancellationToken.None).Result;

//            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
//            var easCsvRecords = _csvParser.GetData(streamReader, new EasCsvRecordMapper()).ToList();
//            var validationErrorModels = _validationService.ValidateData(fileInfo, easCsvRecords);
//            Assert.NotNull(validationErrorModels);
//            Assert.True(validationErrorModels.Count > 0);
//            Assert.Equal(6, validationErrorModels.Count);
//        }

//        [Fact]
//        public void LogValidationErrors()
//        {
//            var fileInfo = new EasFileInfo()
//            {
//                FileName = "EAS-10033670-1819-20180812-100221-05.csv",
//                UKPRN = "10033670",
//                DateTime = DateTime.UtcNow,
//                FilePreparationDate = DateTime.UtcNow.AddHours(-2),
//                FilePath = @"SampleEASFiles\Invalid\EAS-10033670-1819-20180812-100221-05.csv"
//            };

//            var easFileDataProviderService = new EASFileDataProviderService();
//            var streamReader = easFileDataProviderService.Provide(fileInfo, CancellationToken.None).Result;
//            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);

//            var submissionId = Guid.NewGuid();
//            ImportService importService = new ImportService(
//                submissionId,
//                _easSubmissionService,
//                _easPaymentService,
//                easFileDataProviderService,
//                new CsvParser(),
//                _validationService,
//                reportingController.Object,
//                new AzureStorageKeyValuePersistenceService(null),
//                new SeriLogger(new ApplicationLoggerSettings(), new Logging.ExecutionContext(), null));
//            importService.ImportEasDataAsync(fileInfo, CancellationToken.None).GetAwaiter().GetResult();
//        }

//        private static List<FundingLineContractMapping> BuildFundingLineContractMappings()
//        {
//            var fundingLineContractMappings = new List<FundingLineContractMapping>()
//            {
//                new FundingLineContractMapping
//                    { FundingLine = "FundingLine", ContractTypeRequired = "APPS1819" },
//                new FundingLineContractMapping
//                    { FundingLine = "Funding-123+.Line", ContractTypeRequired = "APPS1819" },
//                new FundingLineContractMapping
//                    { FundingLine = "16-18 Apprenticeships", ContractTypeRequired = "APPS1819" },
//                new FundingLineContractMapping
//                    { FundingLine = "19-23 Apprenticeships", ContractTypeRequired = "APPS1819" },
//                new FundingLineContractMapping
//                    { FundingLine = "24+ Apprenticeships", ContractTypeRequired = "APPS1819" },
//                new FundingLineContractMapping
//                    { FundingLine = "19-24 Traineeships (procured from Nov 2017)", ContractTypeRequired = "AEB-TOL" },
//                new FundingLineContractMapping
//                    { FundingLine = "Advanced Learner Loans Bursary", ContractTypeRequired = "ALLB" }
//            };
//            return fundingLineContractMappings;
//        }---

//        private static List<ContractAllocation> BuildContractAllocations()
//        {
//            var contractAllocations = new List<ContractAllocation>()
//            {
//                new ContractAllocation
//                {
//                    FundingStreamPeriodCode = "APPS1819", StartDate = new DateTime(2018, 01, 01),
//                    EndDate = new DateTime(2019, 12, 01)
//                },
//                new ContractAllocation
//                {
//                    FundingStreamPeriodCode = "AEB-TOL", StartDate = new DateTime(2018, 01, 01),
//                    EndDate = new DateTime(2019, 12, 01)
//                },
//            };
//            return contractAllocations;
//        }
//    }
//}
