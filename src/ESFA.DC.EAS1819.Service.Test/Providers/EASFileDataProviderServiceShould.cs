using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Service.Providers;
using Xunit;

namespace ESFA.DC.EAS1819.Service.Test.Providers
{
    public class EASFileDataProviderServiceShould
    {
        [Fact]
        public void ProvideEasRecordsFromAGivenFile()
        {
            var easFileDataProviderService = new EASFileDataProviderService(@"SampleEASFiles\EAS-10033670-1819-20180912-144437-03.csv", default(CancellationToken));
            var easCsvRecords = easFileDataProviderService.Provide().Result;
            Assert.NotNull(easCsvRecords);
            Assert.Equal(2, easCsvRecords.Count);
        }
    }
}
