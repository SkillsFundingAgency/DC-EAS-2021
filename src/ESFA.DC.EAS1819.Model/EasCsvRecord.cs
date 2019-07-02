using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.EAS1819.Model
{
    public class EasCsvRecord
    {
        public string FundingLine { get; set; }

        public string AdjustmentType { get; set; }

        public string CalendarYear { get; set; }

        public string CalendarMonth { get; set; }

        public string Value { get; set; }
    }
}
