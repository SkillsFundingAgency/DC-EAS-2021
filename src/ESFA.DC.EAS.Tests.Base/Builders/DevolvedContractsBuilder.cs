using System;
using System.Collections.Generic;
using ESFA.DC.ReferenceData.FCS.Model;

namespace ESFA.DC.EAS.Tests.Base.Builders
{
    public class DevolvedContractsBuilder
    {
        public static implicit operator Dictionary<string, IEnumerable<DevolvedContract>>(DevolvedContractsBuilder instance)
        {
            return instance.Build();
        }

        public Dictionary<string, IEnumerable<DevolvedContract>> Build()
        {
            return new Dictionary<string, IEnumerable<DevolvedContract>>
            {
                {
                    "London", new List<DevolvedContract>
                    {
                        new DevolvedContract
                        {
                            Ukprn = 10033670,
                            McaglashortCode = "London",
                            EffectiveFrom = new DateTime(2020, 10, 01),
                            EffectiveTo = new DateTime(2021, 12, 01)
                        },
                    }
                },
                {
                    "WMCA", new List<DevolvedContract>
                    {
                        new DevolvedContract
                        {
                            Ukprn = 10033670,
                            McaglashortCode = "WMCA",
                            EffectiveFrom = new DateTime(2019, 10, 01),
                            EffectiveTo = new DateTime(2019, 12, 01)
                        },
                        new DevolvedContract
                        {
                            Ukprn = 10033670,
                            McaglashortCode = "WMCA",
                            EffectiveFrom = new DateTime(2019, 12, 02),
                        },
                    }
                },
                {
                    "GMCA", new List<DevolvedContract>
                    {
                        new DevolvedContract
                        {
                            Ukprn = 10033670,
                            McaglashortCode = "GMCA",
                            EffectiveFrom = new DateTime(2020, 8, 5),
                            EffectiveTo = new DateTime(2021, 7, 31)
                        },
                    }
                },
            };
        }
    }
}
