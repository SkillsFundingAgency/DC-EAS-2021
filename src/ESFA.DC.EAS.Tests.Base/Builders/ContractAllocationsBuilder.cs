using System;
using System.Collections.Generic;
using ESFA.DC.ReferenceData.FCS.Model;

namespace ESFA.DC.EAS.Tests.Base.Builders
{
    public class ContractAllocationsBuilder
    {
        public static implicit operator List<ContractAllocation>(ContractAllocationsBuilder instance)
        {
            return instance.Build();
        }

        public List<ContractAllocation> Build()
        {
            var contractAllocations = new List<ContractAllocation>()
            {
                new ContractAllocation
                {
                    FundingStreamPeriodCode = "APPS1819", StartDate = new DateTime(2018, 01, 01),
                    EndDate = new DateTime(2019, 12, 01)
                },
                new ContractAllocation
                {
                    FundingStreamPeriodCode = "AEB-TOL", StartDate = new DateTime(2018, 01, 01),
                    EndDate = new DateTime(2019, 12, 01)
                },
            };
            return contractAllocations;
        }
    }
}
