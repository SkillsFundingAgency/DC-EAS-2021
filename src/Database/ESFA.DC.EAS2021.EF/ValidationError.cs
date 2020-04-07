using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS2021.EF
{
    public partial class ValidationError
    {
        public int SourceFileId { get; set; }
        public int ValidationErrorId { get; set; }
        public Guid? RowId { get; set; }
        public string RuleId { get; set; }
        public string FundingLine { get; set; }
        public string AdjustmentType { get; set; }
        public string CalendarYear { get; set; }
        public string CalendarMonth { get; set; }
        public string Severity { get; set; }
        public string ErrorMessage { get; set; }
        public string Value { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string DevolvedAreaSoF { get; set; }

        public virtual SourceFile SourceFile { get; set; }
    }
}
