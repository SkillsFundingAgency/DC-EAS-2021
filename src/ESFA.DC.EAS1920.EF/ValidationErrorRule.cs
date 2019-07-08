using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS1920.EF
{
    public partial class ValidationErrorRule
    {
        public string RuleId { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }
        public string SeverityFis { get; set; }
    }
}
