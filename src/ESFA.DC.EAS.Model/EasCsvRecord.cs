using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.EAS.Model
{
    public class EasCsvRecord
    {
        public string FundingLine { get; set; }

        public string AdjustmentType { get; set; }

        public string CalendarYear { get; set; }

        public string CalendarMonth { get; set; }

        public string Value { get; set; }

        public string DevolvedAreaSourceOfFunding { get; set; }
    }
}
