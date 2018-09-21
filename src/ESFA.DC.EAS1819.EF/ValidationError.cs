namespace ESFA.DC.EAS1819.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ValidationError : BaseEntity
    {
        public int SourceFileId { get; set; }

        public int ValidationErrorId { get; set; }

        public Guid? RowId { get; set; }

        public string RuleId { get; set; }

        public string CalendarYear { get; set; }

        public string CalendarMonth { get; set; }

        public string Severity { get; set; }

        public string ErrorMessage { get; set; }

        public string Value { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
