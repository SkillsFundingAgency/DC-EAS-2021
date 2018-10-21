using System;
using System.IO;
using System.Threading;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.Service.Mapper;
using ESFA.DC.EAS1819.Service.Providers;
using Xunit;

namespace ESFA.DC.EAS1819.Service.Test
{
    public class CsvParserShould
    {
        EASFileDataProviderService easFileDataProviderService;
        StreamReader streamReader;

        public CsvParserShould()
        {
            var fileInfo = new EasFileInfo()
            {
                FileName = "EASDATA-10033670-20180912-144437.csv",
                UKPRN = "10033670",
                DateTime = DateTime.UtcNow,
                FilePreparationDate = DateTime.UtcNow.AddHours(-2),
                FilePath = @"SampleEASFiles\Valid\EASDATA-10033670-20180912-144437.csv"
            };
            streamReader = new EASFileDataProviderService().ProvideAsync(fileInfo, CancellationToken.None).Result;
        }

        [Fact]
        public void ReadHeadersFromAStreamReader()
        {
            var sut = new CsvParser();
            var headers = sut.GetHeaders(streamReader);
            Assert.NotNull(headers);
            Assert.Equal(5, headers.Count);
        }

        [Fact]
        public void ReadDataFromAStreamReader()
        {
            var sut = new CsvParser();
            var data = sut.GetData(streamReader, new EasCsvRecordMapper());
            Assert.NotNull(data);
            Assert.Equal(2, data.Count);
        }
    }
}
