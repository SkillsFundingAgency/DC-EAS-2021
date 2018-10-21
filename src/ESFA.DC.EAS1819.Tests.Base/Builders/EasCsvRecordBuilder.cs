using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Model;

namespace ESFA.DC.EAS1819.Tests.Base.Builders
{
    public class EasCsvRecordBuilder
    {
        public static implicit operator List<EasCsvRecord>(EasCsvRecordBuilder instance)
        {
            return instance.Build();
        }

        public List<EasCsvRecord> Build()
        {
            return new List<EasCsvRecord>();
        }

        public List<EasCsvRecord> GetValidRecords()
        {
            var easCsvRecords = new List<EasCsvRecord>()
            {
                new EasCsvRecord()
                {
                    FundingLine = "16-18 Apprenticeships",
                    AdjustmentType = "Excess Learning Support",
                    CalendarYear = 2018,
                    CalendarMonth = 9,
                    Value = (decimal)10000003.68
                },
                new EasCsvRecord()
                {
                    FundingLine = "24+ Apprenticeships",
                    AdjustmentType = "Authorised Claims",
                    CalendarYear = 2018,
                    CalendarMonth = 8,
                    Value = (decimal)12546.99
                }
            };
            return easCsvRecords;
        }

        public List<EasCsvRecord> GetValidAndInvalidRecords()
        {
            var easCsvRecords = new List<EasCsvRecord>()
            {
                new EasCsvRecord()
                {
                    FundingLine = "16-18 Apprenticeships",
                    AdjustmentType = "Excess Learning Support",
                    CalendarYear = 2018,
                    CalendarMonth = 9,
                    Value = (decimal)10000003.68
                },
                new EasCsvRecord()
                {
                    FundingLine = "24+ Apprenticeships",
                    AdjustmentType = "Authorised Claims",
                    CalendarYear = 2018,
                    CalendarMonth = 8,
                    Value = (decimal)12546.99
                },
                new EasCsvRecord()
                {
                    FundingLine = "24+ Apprenticeships",
                    AdjustmentType = "Authorised Claims",
                    CalendarYear = 2018,
                    CalendarMonth = 138, // Invalid Month
                    Value = (decimal)12546.99
                },
                new EasCsvRecord()
                {
                    FundingLine = "24+ Apprenticeships",
                    AdjustmentType = "Authorised Claims",
                    CalendarYear = 2020, // Invalid year and duplicate record
                    CalendarMonth = 138, // Invalid Month
                    Value = (decimal)12546.99
                }
            };
            return easCsvRecords;
        }
    }
}
