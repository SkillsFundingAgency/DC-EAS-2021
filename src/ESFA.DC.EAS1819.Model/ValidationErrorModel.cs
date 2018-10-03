namespace ESFA.DC.EAS1819.Model
{
    public class ValidationErrorModel
    {
        public string FundingLine { get; set; }

        public string AdjustmentType { get; set; }

        public int CalendarYear { get; set; }

        public int CalendarMonth { get; set; }

        public decimal Value { get; set; }

        public string RuleName { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsWarning { get; set; }
}
}
