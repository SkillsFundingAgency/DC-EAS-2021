using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.DataService.FCS;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace ESFA.DC.EAS.DataService.Test
{
    public class FcsDataServiceTests
    {
        [Fact]
        public async Task GetContractsForProviderAsync()
        {
            var cancellationToken = CancellationToken.None;
            var ukprn = 1;

            var fcsContractAllocations = new List<ContractAllocation>()
            {
                new ContractAllocation
                {
                    DeliveryUkprn = 1,
                    FundingStreamPeriodCode = "APPS1819",
                    StartDate = new DateTime(2018, 01, 01),
                    EndDate = new DateTime(2020, 12, 01)
                },
                new ContractAllocation
                {
                    DeliveryUkprn = 1,
                    FundingStreamPeriodCode = "AEB-TOL",
                    StartDate = new DateTime(2018, 01, 01),
                    EndDate = new DateTime(2020, 12, 01)
                },
                new ContractAllocation
                {
                    DeliveryUkprn = 2,
                    FundingStreamPeriodCode = "AEB-TOL",
                    StartDate = new DateTime(2018, 01, 01),
                    EndDate = new DateTime(2020, 12, 01)
                },
            };

            var contextMock = new Mock<IFcsContext>();
            contextMock.Setup(x => x.ContractAllocations).Returns(fcsContractAllocations.AsQueryable().BuildMockDbSet().Object);

            var expectedContractAllocations = new List<ContractAllocation>()
            {
                new ContractAllocation
                {
                    DeliveryUkprn = 1,
                    FundingStreamPeriodCode = "APPS1819",
                    StartDate = new DateTime(2018, 01, 01),
                    EndDate = new DateTime(2020, 12, 01)
                },
                new ContractAllocation
                {
                    DeliveryUkprn = 1,
                    FundingStreamPeriodCode = "AEB-TOL",
                    StartDate = new DateTime(2018, 01, 01),
                    EndDate = new DateTime(2020, 12, 01)
                },
            };

            var serviceResult = await NewService(contextMock.Object).GetContractsForProviderAsync(ukprn, cancellationToken);

            serviceResult.Should().BeEquivalentTo(expectedContractAllocations);
        }


        [Fact]
        public async Task GetDevolvedContractsForProviderAsync()
        {
            var cancellationToken = CancellationToken.None;
            var ukprn = 1;

            var fcsDevolvedContracts = new List<DevolvedContract>()
            {
                new DevolvedContract
                {
                    Ukprn = 1,
                    McaglashortCode = "Code1",
                    EffectiveFrom = new DateTime(2018, 01, 01),
                    EffectiveTo = new DateTime(2020, 12, 01)
                },
                new DevolvedContract
                {
                    Ukprn = 1,
                    McaglashortCode = "Code2",
                    EffectiveFrom = new DateTime(2018, 01, 01),
                    EffectiveTo = new DateTime(2019, 12, 01)
                },
                new DevolvedContract
                {
                    Ukprn = 1,
                    McaglashortCode = "Code2",
                    EffectiveFrom = new DateTime(2019, 12, 02)
                },
                new DevolvedContract
                {
                    Ukprn = 1,
                    McaglashortCode = "CODE1",
                    EffectiveFrom = new DateTime(2020, 12, 02)
                },
                new DevolvedContract
                {
                    Ukprn = 2,
                    McaglashortCode = "Code1",
                    EffectiveFrom = new DateTime(2018, 01, 01),
                    EffectiveTo = new DateTime(2020, 12, 01)
                },
            };

            var contextMock = new Mock<IFcsContext>();
            contextMock.Setup(x => x.DevolvedContracts).Returns(fcsDevolvedContracts.AsQueryable().BuildMockDbSet().Object);

            var expectedDevolvedContracts = new Dictionary<string, IEnumerable<DevolvedContract>>
            {
                {
                    "Code1", new List<DevolvedContract>
                    {
                        new DevolvedContract
                        {
                            Ukprn = 1,
                            McaglashortCode = "Code1",
                            EffectiveFrom = new DateTime(2018, 01, 01),
                            EffectiveTo = new DateTime(2020, 12, 01)
                        },
                        new DevolvedContract
                        {
                            Ukprn = 1,
                            McaglashortCode = "CODE1",
                            EffectiveFrom = new DateTime(2020, 12, 02)
                        },
                    }
                },
                {
                    "Code2", new List<DevolvedContract>
                    {
                        new DevolvedContract
                        {
                            Ukprn = 1,
                            McaglashortCode = "Code2",
                            EffectiveFrom = new DateTime(2018, 01, 01),
                            EffectiveTo = new DateTime(2019, 12, 01)
                        },
                        new DevolvedContract
                        {
                            Ukprn = 1,
                            McaglashortCode = "Code2",
                            EffectiveFrom = new DateTime(2019, 12, 02)
                        },
                    }
                },
            };

            var serviceResult = await NewService(contextMock.Object).GetDevolvedContractsForProviderAsync(ukprn, cancellationToken);

            serviceResult.Should().BeEquivalentTo(expectedDevolvedContracts);
        }

        private FCSDataService NewService(IFcsContext context)
        {
            return new FCSDataService(context);
        }
    }
}
