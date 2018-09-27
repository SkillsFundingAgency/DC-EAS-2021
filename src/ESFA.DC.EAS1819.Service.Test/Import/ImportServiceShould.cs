using System.Threading;
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
        [Fact]
        public void ImportEasCsv()
        {
            var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];
            IRepository<EasSubmission> easSubmissionRepository = new Repository<EasSubmission>(context: new EasdbContext(connString));
            IRepository<EasSubmissionValues> easSubmissionValuesRepository = new Repository<EasSubmissionValues>(context: new EasdbContext(connString));

            IRepository<PaymentTypes> paymentRepository = new Repository<PaymentTypes>(context: new EasdbContext(connString));
            EasSubmissionService easSubmissionService = new EasSubmissionService(easSubmissionRepository, easSubmissionValuesRepository);
            EasPaymentService easPaymentService = new EasPaymentService(paymentRepository);

            var easFileDataProviderService = new EASFileDataProviderService(@"SampleEASFiles\EAS-10033670-1819-20180912-144437-03.csv", new EasValidationService(easPaymentService, new DateTimeProvider.DateTimeProvider()), new CsvParser(),   default(CancellationToken));

            var submissionId = Guid.NewGuid();
            ImportService importService = new ImportService(submissionId, easSubmissionService, easPaymentService, easFileDataProviderService);
            importService.ImportEasData();
            var easSubmissionValues = easSubmissionService.GetEasSubmissionValues(submissionId);
            Assert.NotEmpty(easSubmissionValues);
            Assert.Equal(2, easSubmissionValues.Count);
        }
    }
}
