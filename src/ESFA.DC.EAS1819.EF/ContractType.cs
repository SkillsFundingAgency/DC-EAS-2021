namespace ESFA.DC.EAS1819.EF
{
    using System.Collections.Generic;

    public partial class ContractType : BaseEntity
    {
       public ContractType()
        {
            FundingLines = new HashSet<FundingLine>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<FundingLine> FundingLines { get; set; }
    }
}
