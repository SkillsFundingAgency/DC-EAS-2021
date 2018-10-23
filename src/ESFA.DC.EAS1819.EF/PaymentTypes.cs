namespace ESFA.DC.EAS1819.EF
{
    public partial class PaymentTypes : BaseEntity
    {
        public int PaymentId { get; set; }

        public string PaymentName { get; set; }

        public bool FM36 { get; set; }

        public string PaymentTypeDescription { get; set; }

        public int? FundingLineId { get; set; }

        public int? AdjustmentTypeId { get; set; }

        public virtual AdjustmentType AdjustmentType { get; set; }

        public virtual FundingLine FundingLine { get; set; }
    }
}
