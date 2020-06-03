using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.Service.Mapper;
using ESFA.DC.EAS.Service.Providers;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.EAS.Service.Test
{
    public class EASFileDataProviderServiceTests
    {
        [Fact]
        public async Task ProvideData()
        {
            var filename = "FileName";
            var container = "Container";
            var cancellationToken = CancellationToken.None;

            var models = new List<EasCsvRecord>
            {
                new EasCsvRecord
                {
                    FundingLine = "FundingLine"
                },
                new EasCsvRecord
                {
                    FundingLine = "FundingLine"
                }
            };

            var csvService = new Mock<ICsvFileService>();
            csvService.Setup(x => x.ReadAllAsync<EasCsvRecord, EasCsvRecordMapper>(filename, container, cancellationToken)).ReturnsAsync(models);

            var service = new EASFileDataProviderService(csvService.Object);

            var result = await service.ProvideData(filename, container, cancellationToken);

            result.Should().BeEquivalentTo(models);
        }
    }
}
