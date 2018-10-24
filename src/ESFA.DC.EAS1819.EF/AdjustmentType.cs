namespace ESFA.DC.EAS1819.EF
{
    using System.Collections.Generic;

    public partial class AdjustmentType : BaseEntity
    {
        public AdjustmentType()
        {
            PaymentTypes = new HashSet<PaymentTypes>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<PaymentTypes> PaymentTypes { get; set; }
    }
}
