using ESFA.DC.EAS1819.Model.Enums;
using ESFA.DC.EAS1819.Model.Extensions;

namespace ESFA.DC.EAS1819.Model
{
    public class Payment
    {
        PaymentType type;

        public PaymentType Type
        {
            get { return type; }

            set
            {
                type = value;
                PaymentTypeDescription = value.ToStringEnums();
                ValueFormatString = (type.Equals(PaymentType.AuditAdjustments) || type.Equals(PaymentType.AuthorisedClaims) || type.Equals(PaymentType.AreaCostsAuditAdjustments)) ? "8.2" : "6.2";
            }
        }

        public string PaymentTypeDescription { get; set; }

        public decimal? Value { get; set; }

        public bool ReadOnly { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsInvalid { get; set; }

        public bool Changed { get; set; }

        public string FieldId { get; set; }

        public string ValueFormatString { get; set; }
    }
}