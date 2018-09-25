﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.EAS1819.Model
{
    public class EasCsvRecord
    {
        public string FundingLine { get; set; }

        public string AdjustmentType { get; set; }

        public int CalendarYear { get; set; }

        public int CalendarMonth { get; set; }

        public decimal Value { get; set; }
    }
}
