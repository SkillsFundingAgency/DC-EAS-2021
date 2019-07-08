using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS1920.EF
{
    public partial class EasSubmission
    {
        public virtual ICollection<EasSubmissionValue> EasSubmissionValues { get; set; }
    }
}
