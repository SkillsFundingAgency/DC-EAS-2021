using System.Linq;
using System.Threading;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.Service.Mapper;
using ESFA.DC.EAS1819.Service.Providers;
using ESFA.DC.EAS1819.Service.Validation;

namespace ESFA.DC.EAS1819.Service.Test.Import
{
    using System;
    using System.Configuration;
    using System.IO;
    using ESESFA.DC.EAS1819.DataService;
    using ESFA.DC.EAS1819.DataService;
    using ESFA.DC.EAS1819.DataService.Interface;
    using ESFA.DC.EAS1819.EF;
    using ESFA.DC.EAS1819.Service.Import;
    using Xunit;

    public class ImportServiceShould
    {
        EasValidationService _validationService;
        EasSubmissionService _easSubmissionService;
        EasPaymentService _easPaymentService;
        CsvParser _csvParser;

        public ImportServiceShould()
        {
            var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];
            _csvParser = new CsvParser();
            IRepository<PaymentTypes> paymentRepository = new Repository<PaymentTypes>(context: new EasdbContext(connString));
            _easPaymentService = new EasPaymentService(paymentRepository);
            IRepository<EasSubmission> easSubmissionRepository = new Repository<EasSubmission>(context: new EasdbContext(connString));
            IRepository<EasSubmissionValues> easSubmissionValuesRepository = new Repository<EasSubmissionValues>(context: new EasdbContext(connString));
            IRepository<ValidationError> validationErrorRepo = new Repository<ValidationError>(context: new EasdbContext(connString));
            IRepository<SourceFile> sourceFileRepo = new Repository<SourceFile>(context: new EasdbContext(connString));
            _easSubmissionService = new EasSubmissionService(easSubmissionRepository, easSubmissionValuesRepository);
            ValidationErrorService valdiationErrorService = new ValidationErrorService(validationErrorRepo, sourceFileRepo);
            _validationService = new EasValidationService(_easPaymentService, new DateTimeProvider.DateTimeProvider(), valdiationErrorService);
        }

        [Fact]
        public void ImportEasCsv()
        {
            var fileInfo = new EasFileInfo()
            {
                FileName = "EAS-10033670-1819-20180912-144437-03.csv",
                UKPRN = "10033670",
                DateTime = DateTime.UtcNow,
                FilePreparationDate = DateTime.UtcNow.AddHours(-2),
                FilePath = @"SampleEASFiles\Valid\EAS-10033670-1819-20180912-144437-03.csv"
            };
            //IJobContextMessage jobContextMessage = new JobContextMessage()
            //{
            //    JobId = 100,
            //    KeyValuePairs = new Dictionary<string, object>()
            //    {
            //        { "Filename", "EASDATA-12345678-20180924-100516.csv" }
            //    },
            //    SubmissionDateTimeUtc = DateTime.UtcNow,
            //    TopicPointer = 1,
            //    Topics = new ArraySegment<ITopicItem>()
            //};
            var easFileDataProviderService = new EASFileDataProviderService();
            var submissionId = Guid.NewGuid();
            ImportService importService = new ImportService(
                                                            submissionId,
                                                            _easSubmissionService,
                                                            _easPaymentService,
                                                            easFileDataProviderService,
                                                            new CsvParser(),
                                                            _validationService);
            importService.ImportEasData(fileInfo);
            var easSubmissionValues = _easSubmissionService.GetEasSubmissionValues(submissionId);
            Assert.NotEmpty(easSubmissionValues);
            Assert.Equal(2, easSubmissionValues.Count);
        }

        [Fact]
        public void ValidateEasRecords()
        {
            var fileInfo = new EasFileInfo()
            {
                FileName = "EAS-10033670-1819-20180812-100221-05.csv",
                UKPRN = "10033670",
                DateTime = DateTime.UtcNow,
                FilePreparationDate = DateTime.UtcNow.AddHours(-2),
                FilePath = @"SampleEASFiles\Invalid\EAS-10033670-1819-20180812-100221-05.csv"
            };

            var easFileDataProviderService = new EASFileDataProviderService();
            var streamReader = easFileDataProviderService.Provide(fileInfo).Result;

            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            var easCsvRecords = _csvParser.GetData(streamReader, new EasCsvRecordMapper()).ToList();
            var validationErrorModels = _validationService.ValidateData(easCsvRecords);
            //var generateViolationReport = _validationService.GenerateViolationReport(validationErrorModels);
            Assert.NotNull(validationErrorModels);
            Assert.True(validationErrorModels.Count > 0);
            Assert.Equal(6, validationErrorModels.Count);
        }

        [Fact]
        public void LogValidationErrors()
        {
            var fileInfo = new EasFileInfo()
            {
                FileName = "EAS-10033670-1819-20180812-100221-05.csv",
                UKPRN = "10033670",
                DateTime = DateTime.UtcNow,
                FilePreparationDate = DateTime.UtcNow.AddHours(-2),
                FilePath = @"SampleEASFiles\Invalid\EAS-10033670-1819-20180812-100221-05.csv"
            };

            var easFileDataProviderService = new EASFileDataProviderService();
            var streamReader = easFileDataProviderService.Provide(fileInfo).Result;
            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);

            var submissionId = Guid.NewGuid();
            ImportService importService = new ImportService(
                submissionId,
                _easSubmissionService,
                _easPaymentService,
                easFileDataProviderService,
                new CsvParser(),
                _validationService);
            importService.ImportEasData(fileInfo);
        }
    }
}
