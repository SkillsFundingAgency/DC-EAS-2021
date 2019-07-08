namespace ESFA.DC.EAS.Model
{
    public class ValidationErrorModel
    {
        public string FundingLine { get; set; }

        public string AdjustmentType { get; set; }

        public string CalendarYear { get; set; }

        public string CalendarMonth { get; set; }

        public string Value { get; set; }

        public string RuleName { get; set; }

        public string ErrorMessage { get; set; }

        public string Severity { get; set; }

        public string OfficialSensitive { get; set; }
    }
}
