using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS1819.Model
{
    public class EAS1718Model
    {
        public string UKPRN { get; set; }

        public string ProviderName { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public bool Declaration { get; set; }

        public List<Period> Periods { get; set; }
    }
}