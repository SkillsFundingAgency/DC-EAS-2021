using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Model;

namespace ESFA.DC.EAS1819.ValidationService.Mapper
{
    public sealed class EasCsvRecordMapper : ClassMap<EasCsvRecord>, IClassMapper
    {
        public EasCsvRecordMapper()
        {
            Map(m => m.FundingLine).Index(0).Name("FundingLine");
            Map(m => m.AdjustmentType).Index(1).Name("AdjustmentType");
            Map(m => m.CalendarYear).Index(2).Name("CalendarYear");
            Map(m => m.CalendarMonth).Index(3).Name("CalendarMonth");
            Map(m => m.Value).Index(4).Name("Value").Default((decimal?)null);
        }
    }
}
