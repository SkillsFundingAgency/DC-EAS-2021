using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS1920.EF
{
    public partial class EasSubmissionValue
    {
        public Guid SubmissionId { get; set; }
        public int CollectionPeriod { get; set; }
        public int PaymentId { get; set; }
        public decimal PaymentValue { get; set; }
        public int DevolvedAreaSoF { get; set; }

        public virtual EasSubmission EasSubmission { get; set; }
        public virtual PaymentType Payment { get; set; }
    }
}
