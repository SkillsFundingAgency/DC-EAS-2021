using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESESFA.DC.EAS1819.DataService;
using ESFA.DC.EAS1819.DataService;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Service.Providers;
using ESFA.DC.EAS1819.Service.Validation;
using Xunit;

namespace ESFA.DC.EAS1819.Service.Test.Providers
{
    public class EASFileDataProviderServiceShould
    {
        [Fact]
        public void ProvideEasRecordsFromAGivenFile()
        {
            var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];
            IRepository<EasSubmission> easSubmissionRepository = new Repository<EasSubmission>(context: new EasdbContext(connString));
            IRepository<EasSubmissionValues> easSubmissionValuesRepository = new Repository<EasSubmissionValues>(context: new EasdbContext(connString));

            IRepository<PaymentTypes> paymentRepository = new Repository<PaymentTypes>(context: new EasdbContext(connString));
            EasSubmissionService easSubmissionService = new EasSubmissionService(easSubmissionRepository, easSubmissionValuesRepository);
            EasPaymentService easPaymentService = new EasPaymentService(paymentRepository);

            var easFileDataProviderService = new EASFileDataProviderService(
                @"SampleEASFiles\EAS-10033670-1819-20180912-144437-03.csv",
                new EasValidationService(easPaymentService, new DateTimeProvider.DateTimeProvider()),
                new CsvParser(),
                default(CancellationToken));
            var easCsvRecords = easFileDataProviderService.Provide().Result;
            Assert.NotNull(easCsvRecords);
            Assert.Equal(2, easCsvRecords.Count);
        }
    }
}
