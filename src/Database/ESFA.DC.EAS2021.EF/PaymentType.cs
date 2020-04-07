using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS2021.EF
{
    public partial class PaymentType
    {
        public PaymentType()
        {
            EasSubmissionValues = new HashSet<EasSubmissionValue>();
        }

        public int PaymentId { get; set; }
        public string PaymentName { get; set; }
        public bool Fm36 { get; set; }
        public string PaymentTypeDescription { get; set; }
        public int? FundingLineId { get; set; }
        public int? AdjustmentTypeId { get; set; }

        public virtual AdjustmentType AdjustmentType { get; set; }
        public virtual FundingLine FundingLine { get; set; }
        public virtual ICollection<EasSubmissionValue> EasSubmissionValues { get; set; }
    }
}
