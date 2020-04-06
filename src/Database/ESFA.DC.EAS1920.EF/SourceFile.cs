using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS1920.EF
{
    public partial class SourceFile
    {
        public SourceFile()
        {
            ValidationErrors = new HashSet<ValidationError>();
        }

        public int SourceFileId { get; set; }
        public string FileName { get; set; }
        public DateTime FilePreparationDate { get; set; }
        public string Ukprn { get; set; }
        public DateTime? DateTime { get; set; }

        public virtual ICollection<ValidationError> ValidationErrors { get; set; }
    }
}
