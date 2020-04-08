using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS2021.EF
{
    public partial class AdjustmentType
    {
        public AdjustmentType()
        {
            PaymentTypes = new HashSet<PaymentType>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<PaymentType> PaymentTypes { get; set; }
    }
}
