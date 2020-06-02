using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.DataService.Constants;
using ESFA.DC.EAS.DataService.Postcodes;
using ESFA.DC.ReferenceData.Postcodes.Model;
using ESFA.DC.ReferenceData.Postcodes.Model.Interface;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace ESFA.DC.EAS.DataService.Test
{
    public class PostcodesDataServiceTests
    {
        [Fact]
        public async Task GetMcaShortCodesForSofCodes()
        {
            var cancellationToken = CancellationToken.None;

            var contextMock = new Mock<IPostcodesContext>();
            contextMock.Setup(x => x.McaglaSofs).Returns(TestMcaSofs().AsQueryable().BuildMockDbSet().Object);

            var expectedResult = new Dictionary<int, string>
            {
                { 110, "GMCA" },
                { 111, "LCRCA" },
                { 112, "WMCA" },
                { 113, "WECA" },
                { 114, "TVCA" },
                { 115, "CPCA" },
                { 116, "London" },
                { 117, "NTCA" },
            };

            var result = await NewService(contextMock.Object).GetMcaShortCodesForSofCodes(DataServiceConstants.ValidDevolvedSourceOfFundingCodes, cancellationToken);

            Assert.Equal(result, expectedResult);
        }

        private ICollection<McaglaSof> TestMcaSofs()
        {
            return new List<McaglaSof>
            {
                new McaglaSof { McaglaShortCode = "CPCA", SofCode = "115", EffectiveFrom = new DateTime(2019, 8, 1) },
                new McaglaSof { McaglaShortCode = "EnglandND", SofCode = "105", EffectiveFrom = new DateTime(2019, 8, 1) },
                new McaglaSof { McaglaShortCode = "GMCA", SofCode = "110", EffectiveFrom = new DateTime(2019, 8, 1) },
                new McaglaSof { McaglaShortCode = "LCRCA", SofCode = "111", EffectiveFrom = new DateTime(2019, 8, 1) },
                new McaglaSof { McaglaShortCode = "London", SofCode = "116", EffectiveFrom = new DateTime(2019, 8, 1) },
                new McaglaSof { McaglaShortCode = "NTCA", SofCode = "105", EffectiveFrom = new DateTime(2019, 8, 1), EffectiveTo = new DateTime(2020, 7, 31) },
                new McaglaSof { McaglaShortCode = "NTCA", SofCode = "117", EffectiveFrom = new DateTime(2020, 8, 1) },
                new McaglaSof { McaglaShortCode = "Scotland", EffectiveFrom = new DateTime(2019, 08, 01) },
                new McaglaSof { McaglaShortCode = "SCRCA", SofCode = "105", EffectiveFrom = new DateTime(2019, 8, 1), EffectiveTo = new DateTime(2021, 7, 31) },
                new McaglaSof { McaglaShortCode = "TVCA", SofCode = "114", EffectiveFrom = new DateTime(2019, 8, 1) },
                new McaglaSof { McaglaShortCode = "Wales", EffectiveFrom = new DateTime(2019, 8, 1) },
                new McaglaSof { McaglaShortCode = "WECA", SofCode = "113", EffectiveFrom = new DateTime(2019, 08, 01) },
                new McaglaSof { McaglaShortCode = "WMCA", SofCode = "112", EffectiveFrom = new DateTime(2019, 08, 01) }
            };
        }

        private PostcodesDataService NewService(IPostcodesContext postcodesContext)
        {
            return new PostcodesDataService(postcodesContext);
        }
    }
}
