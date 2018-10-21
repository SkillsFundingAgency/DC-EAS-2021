using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.ReferenceData.FCS.Model;

namespace ESFA.DC.EAS1819.Tests.Base.Builders
{
    public class FundingLineContractMappingsBuilder
    {
        public static implicit operator List<FundingLineContractMapping>(FundingLineContractMappingsBuilder instance)
        {
            return instance.Build();
        }

        public List<FundingLineContractMapping> Build()
        {
            var fundingLineContractMappings = new List<FundingLineContractMapping>()
            {
                new FundingLineContractMapping
                    { FundingLine = "FundingLine", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping
                    { FundingLine = "Funding-123+.Line", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping
                    { FundingLine = "16-18 Apprenticeships", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping
                    { FundingLine = "19-23 Apprenticeships", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping
                    { FundingLine = "24+ Apprenticeships", ContractTypeRequired = "APPS1819" },
                new FundingLineContractMapping
                    { FundingLine = "19-24 Traineeships (procured from Nov 2017)", ContractTypeRequired = "AEB-TOL" },
                new FundingLineContractMapping
                    { FundingLine = "Advanced Learner Loans Bursary", ContractTypeRequired = "ALLB" }
            };
            return fundingLineContractMappings;
        }
    }
}
