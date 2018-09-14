using System;

namespace ESFA.DC.EAS1819.EF
{
    public partial class EasSubmissionValues : BaseEntity
    {
        public Guid SubmissionId { get; set; }

        public int CollectionPeriod { get; set; }

        public int PaymentId { get; set; }

        public decimal PaymentValue { get; set; }
    }
}
