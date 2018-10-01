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
using ESFA.DC.EAS1819.Service.Mapper;
using ESFA.DC.EAS1819.Service.Providers;
using Xunit;

namespace ESFA.DC.EAS1819.Service.Test
{
    public class CsvParserShould
    {
        EASFileDataProviderService easFileDataProviderService;

        public CsvParserShould()
        {
            var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];
            IRepository<PaymentTypes> paymentRepository = new Repository<PaymentTypes>(context: new EasdbContext(connString));
            EasPaymentService easPaymentService = new EasPaymentService(paymentRepository);
            easFileDataProviderService = new EASFileDataProviderService(
                @"SampleEASFiles\EAS-10033670-1819-20180912-144437-03.csv",
                new EasValidationService(easPaymentService, new DateTimeProvider.DateTimeProvider()),
                new CsvParser(),
                default(CancellationToken));
        }

        [Fact]
        public void ReadHeadersFromAStreamReader()
        {
           var streamReader = easFileDataProviderService.Provide().Result;
            var sut = new CsvParser();
            var headers = sut.GetHeaders(streamReader);
            Assert.NotNull(headers);
            Assert.Equal(5, headers.Count);
        }

        [Fact]
        public void ReadDataFromAStreamReader()
        {
            var sut = new CsvParser();
            var streamReader = easFileDataProviderService.Provide().Result;
            var data = sut.GetData(streamReader, new EasCsvRecordMapper());
            Assert.NotNull(data);
            Assert.Equal(2, data.Count);
        }
    }
}
