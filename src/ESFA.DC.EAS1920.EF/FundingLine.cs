using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS1920.EF
{
    public partial class FundingLine
    {
        public FundingLine()
        {
            FundingLineContractTypeMappings = new HashSet<FundingLineContractTypeMapping>();
            FundingLineDevolvedAreaSoFmappings = new HashSet<FundingLineDevolvedAreaSoFmapping>();
            PaymentTypes = new HashSet<PaymentType>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<FundingLineContractTypeMapping> FundingLineContractTypeMappings { get; set; }
        public virtual ICollection<FundingLineDevolvedAreaSoFmapping> FundingLineDevolvedAreaSoFmappings { get; set; }
        public virtual ICollection<PaymentType> PaymentTypes { get; set; }
    }
}
