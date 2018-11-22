namespace ESFA.DC.EAS1819.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ValidationErrorRule : BaseEntity
    {
        public string RuleId { get; set; }

        public string Severity { get; set; }

        public string Message { get; set; }

        public string SeverityFIS { get; set; }
    }
}
