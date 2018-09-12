using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.Services.Interface;

namespace ESFA.DC.EAS1819.Services.Mapper
{
    public sealed class EasCsvRecordMapper : ClassMap<EasCsvRecord>, IClassMapper
    {
        public EasCsvRecordMapper()
        {
            Map(m => m.FundingLine).Index(0).Name("Funding Line");
            Map(m => m.EarningsAdjustment).Index(1).Name("Earnings Adjustment");
            Map(m => m.Month).Index(2).Name("Calendar Month");
            Map(m => m.Value).Index(3).Name("Value");
        }
    }
}
