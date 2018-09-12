using System.Collections.Generic;

namespace ESFA.DC.EAS1819.Model
{
    public class PaymentRow
    {
        public string RowHeading { get; set; }

        public List<Payment> Payments { get; set; }

        public int DisplayOrder { get; set; }
    }
}