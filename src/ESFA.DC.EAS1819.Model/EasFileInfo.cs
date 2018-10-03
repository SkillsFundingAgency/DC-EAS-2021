﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.EAS1819.Model
{
    public class EasFileInfo
    {
        public string FileName { get; set; }

        public string UKPRN { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime FilePreparationDate { get; set; }
    }
}
