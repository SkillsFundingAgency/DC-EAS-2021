using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.DataService;
using ESFA.DC.EAS1819.DataService;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.Service.Mapper;
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
            var fileInfo = new EasFileInfo()
            {
                FileName = "EAS-10033670-1819-20180912-144437-03.csv",
                UKPRN = "10033670",
                DateTime = DateTime.UtcNow,
                FilePreparationDate = DateTime.UtcNow.AddHours(-2),
                FilePath = @"SampleEASFiles\Valid\EAS-10033670-1819-20180912-144437-03.csv"
            };

            var easFileDataProviderService = new EASFileDataProviderService();
            var streamReader = easFileDataProviderService.Provide(fileInfo, CancellationToken.None).Result;
            CsvParser csvParser = new CsvParser();
            var headers = csvParser.GetHeaders(streamReader);
            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            var easCsvRecords = csvParser.GetData(streamReader, new EasCsvRecordMapper());

            Assert.NotNull(easCsvRecords);
            Assert.Equal(2, easCsvRecords.Count);
        }
    }
}
