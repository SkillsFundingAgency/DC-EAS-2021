using System.Collections.Generic;
using ESFA.DC.EAS1819.Model.Enums;

namespace ESFA.DC.EAS1819.Model
{
    public class SubSection
    {
        public string SubSectionHeading { get; set; }

        public List<PaymentRow> PaymentRows { get; set; }

        public SectionType Type { get; set; }
    }
}