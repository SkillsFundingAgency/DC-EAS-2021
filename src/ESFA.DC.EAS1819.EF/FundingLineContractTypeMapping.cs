using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS1819.EF
{
    public partial class FundingLineContractTypeMapping
    {
        public int FundingLineId { get; set; }
        public int ContractTypeId { get; set; }

        public virtual ContractType ContractType { get; set; }
        public virtual FundingLine FundingLine { get; set; }
    }
}
