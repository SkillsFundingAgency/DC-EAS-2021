using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.EAS1819.Model
{
    public class EasCsvRecord
    {
        public string FundingLine { get; set; }

        public string EarningsAdjustment { get; set; }

        public int Month { get; set; }

        public decimal Value { get; set; }
    }
}
