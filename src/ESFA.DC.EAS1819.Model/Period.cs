using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS1819.Model
{
    public class Period
    {
        public string PeriodIdentifier { get; set; }

        public string PeriodName { get; set; }

        public DateTime PeriodStartDate { get; set; }

        public DateTime PeriodEndDate { get; set; }

        public List<Section> Sections { get; set; }

        public bool IsCurrentPeriod { get; set; }
    }
}