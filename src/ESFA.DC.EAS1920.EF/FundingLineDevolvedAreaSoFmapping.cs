﻿using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS1920.EF
{
    public partial class FundingLineDevolvedAreaSoFmapping
    {
        public int FundingLineId { get; set; }
        public int DevolvedAreaSoF { get; set; }

        public virtual FundingLine FundingLine { get; set; }
    }
}
