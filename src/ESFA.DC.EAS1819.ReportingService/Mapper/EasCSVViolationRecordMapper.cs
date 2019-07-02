using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Model;

namespace ESFA.DC.EAS1819.ReportingService.Mapper
{
    public sealed class EasCsvViolationRecordMapper : ClassMap<ValidationErrorModel>, IClassMapper
    {
        public EasCsvViolationRecordMapper()
        {
            Map(m => m.Severity).Name("Error/Warning");
            Map(m => m.RuleName).Name("Rule name");
            Map(m => m.ErrorMessage).Name("Error Message");
            Map(m => m.FundingLine).Name("FundingLine");
            Map(m => m.AdjustmentType).Name("AdjustmentType");
            Map(m => m.CalendarYear).Name("CalendarYear");
            Map(m => m.CalendarMonth).Name("CalendarMonth");
            Map(m => m.Value).Name("Value");
            Map(m => m.OfficialSensitive).Name("OFFICIAL-SENSITIVE");
        }
    }
}
