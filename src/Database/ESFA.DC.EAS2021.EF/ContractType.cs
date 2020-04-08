using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS2021.EF
{
    public partial class ContractType
    {
        public ContractType()
        {
            FundingLineContractTypeMappings = new HashSet<FundingLineContractTypeMapping>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<FundingLineContractTypeMapping> FundingLineContractTypeMappings { get; set; }
    }
}
