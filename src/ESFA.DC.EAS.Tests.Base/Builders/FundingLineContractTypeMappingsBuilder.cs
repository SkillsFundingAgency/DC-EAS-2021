using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.ReferenceData.FCS.Model;

namespace ESFA.DC.EAS.Tests.Base.Builders
{
    public class FundingLineContractTypeMappingsBuilder
    {
        public static implicit operator List<FundingLineContractTypeMapping>(FundingLineContractTypeMappingsBuilder instance)
        {
            return instance.Build();
        }

        public List<FundingLineContractTypeMapping> Build()
        {
            var fundingLineContractMappings = new List<FundingLineContractTypeMapping>()
            {
                new FundingLineContractTypeMapping
                    { FundingLine = new FundingLine { Id = 1, Name = "FundingLine1920" }, ContractType = new ContractType { Id = 1, Name = "APPS1920" } },
                new FundingLineContractTypeMapping
                    { FundingLine = new FundingLine { Id = 1, Name = "FundingLine" }, ContractType = new ContractType { Id = 1, Name = "APPS1819" } },
                new FundingLineContractTypeMapping
                        { FundingLine = new FundingLine { Id = 2, Name = "Funding-123+.Line" }, ContractType = new ContractType { Id = 1, Name = "APPS1819" } },
                new FundingLineContractTypeMapping
                    { FundingLine = new FundingLine { Id = 3, Name = "16-18 Apprenticeships" }, ContractType = new ContractType { Id = 1, Name = "APPS1819" } },
                new FundingLineContractTypeMapping
                    { FundingLine = new FundingLine { Id = 4, Name = "19-23 Apprenticeships" }, ContractType = new ContractType { Id = 1, Name = "APPS1819" } },
                new FundingLineContractTypeMapping
                    { FundingLine = new FundingLine { Id = 5, Name = "24+ Apprenticeships" }, ContractType = new ContractType { Id = 1, Name = "APPS1819" } },
                new FundingLineContractTypeMapping
                    { FundingLine = new FundingLine { Id = 6, Name = "19-24 Traineeships (procured from Nov 2017)" }, ContractType = new ContractType() { Id = 2, Name = "AEB-TOL" } },
                new FundingLineContractTypeMapping
                    { FundingLine = new FundingLine { Id = 7, Name = "Advanced Learner Loans Bursary" }, ContractType = new ContractType { Id = 3, Name = "ALLB" } }
            };
            return fundingLineContractMappings;
        }
    }
}
