using System.Collections.Generic;

namespace ESFA.DC.EAS1920.EF
{
    public partial class SourceFile
    {
        public virtual ICollection<ValidationError> ValidationErrors { get; set; }
    }
}
