using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.EAS1819.Model
{
    public class EasCsvData
    {
        public IList<string> Headers { get; set; }

        public IList<EasCsvRecord> Records { get; set; }
    }
}
