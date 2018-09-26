namespace ESFA.DC.EAS1819.EF
{
    public partial class PaymentTypes : BaseEntity
    {
        public int PaymentId { get; set; }

        public string PaymentName { get; set; }

        public bool FM36 { get; set; }

        public string SubSectionHeading { get; set; }

        public string RowHeading { get; set; }

        public string PaymentTypeDescription { get; set; }

        public string FundingLine { get; set; }

        public string AdjustmentType { get; set; }
    }
}
