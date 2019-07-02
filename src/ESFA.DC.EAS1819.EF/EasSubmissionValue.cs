using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS1819.EF
{
    public partial class EasSubmissionValue
    {
        public Guid SubmissionId { get; set; }
        public int CollectionPeriod { get; set; }
        public int PaymentId { get; set; }
        public decimal PaymentValue { get; set; }

        public virtual EasSubmission EasSubmission { get; set; }
    }
}
