using System.IO;
using System.Threading;
using ESFA.DC.EAS.Service.Mapper;
using ESFA.DC.EAS.Service.Providers;
using ESFA.DC.EAS.Tests.Base.Builders;
using Xunit;

namespace ESFA.DC.EAS.Service.Test.Providers
{
    public class EASFileDataProviderServiceShould
    {
        [Fact]
        public void ProvideEasRecordsFromAGivenFile()
        {
            var fileInfo = new EasFileInfoBuilder().WithFileName("EASDATA-10033670-20180909-090909.csv").WithFilePath(@"SampleEASFiles\Mixed\EASDATA-10033670-20180909-090909.csv").Build();

            var easFileDataProviderService = new EASFileDataProviderService();
            var streamReader = easFileDataProviderService.ProvideAsync(fileInfo, CancellationToken.None).Result;
            CsvParser csvParser = new CsvParser();
            var headers = csvParser.GetHeaders(streamReader);
            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            var easCsvRecords = csvParser.GetData(streamReader, new EasCsvRecordMapper());

            Assert.NotNull(easCsvRecords);
            Assert.Equal(4, easCsvRecords.Count);
        }
    }
}
