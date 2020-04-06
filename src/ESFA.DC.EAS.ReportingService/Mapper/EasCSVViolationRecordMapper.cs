using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Model;

namespace ESFA.DC.EAS.ReportingService.Mapper
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
            Map(m => m.DevolvedAreaSoF).Name("DevolvedAreaSourceOfFunding");
            Map(m => m.OfficialSensitive).Name("OFFICIAL-SENSITIVE");
        }
    }
}
