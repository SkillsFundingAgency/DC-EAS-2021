using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS1819.EF
{
    public partial class EasSubmission
    {
        public EasSubmission()
        {
            EasSubmissionValues = new HashSet<EasSubmissionValue>();
        }

        public Guid SubmissionId { get; set; }
        public string Ukprn { get; set; }
        public int CollectionPeriod { get; set; }
        public string ProviderName { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool DeclarationChecked { get; set; }
        public bool NilReturn { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<EasSubmissionValue> EasSubmissionValues { get; set; }
    }
}
