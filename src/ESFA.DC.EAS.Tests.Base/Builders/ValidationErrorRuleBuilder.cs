using System.Collections.Generic;
using ESFA.DC.EAS1920.EF;

namespace ESFA.DC.EAS.Tests.Base.Builders
{
    public class ValidationErrorRuleBuilder
    {
        public static implicit operator List<ValidationErrorRule>(ValidationErrorRuleBuilder instance)
        {
            return instance.Build();
        }

        public List<ValidationErrorRule> Build()
        {
            return GetValidationErrorRules();
        }

        public List<ValidationErrorRule> GetValidationErrorRules()
        {
            var rules = new List<ValidationErrorRule>()
            {
                new ValidationErrorRule { RuleId = "AdjustmentType_01", Severity = "E", Message = "The AdjustmentType is not valid" },
                new ValidationErrorRule { RuleId = "AdjustmentType_02", Severity = "E", Message = "The claimed adjustment must be valid for the funding line" },
                new ValidationErrorRule { RuleId = "CalendarMonth_01", Severity = "E", Message = "The CalendarMonth is not valid." },
                new ValidationErrorRule { RuleId = "CalendarYear_01", Severity = "E", Message = "The CalendarYear is not valid" },
                new ValidationErrorRule { RuleId = "CalendarYearCalendarMonth_01", Severity = "E", Message = "The CalendarMonth you have submitted data for cannot be in the future." },
                new ValidationErrorRule { RuleId = "CalendarYearCalendarMonth_02", Severity = "E", Message = "The CalendarMonth / year you have submitted data for is not within this academic year." },
                new ValidationErrorRule { RuleId = "Duplicate_01", Severity = "E", Message = "This record is a duplicate." },
                new ValidationErrorRule { RuleId = "Fileformat_01", Severity = "E", Message = "The file format is incorrect.  Please check the field headers are as per the Guidance document." },
                new ValidationErrorRule { RuleId = "Filename_01", Severity = "E", Message = "The UKPRN in the filename does not match the UKPRN in Submit Learner Data" },
                new ValidationErrorRule { RuleId = "Filename_02", Severity = "E", Message = "The UKPRN in the filename is invalid" },
                new ValidationErrorRule { RuleId = "Filename_03", Severity = "E", Message = "The filename is not in the correct format" },
                new ValidationErrorRule { RuleId = "Filename_04", Severity = "E", Message = "A file with this filename has already been processed" },
                new ValidationErrorRule { RuleId = "Filename_05", Severity = "E", Message = "The date/time in the filename cannot be after the current date/time" },
                new ValidationErrorRule { RuleId = "Filename_07", Severity = "E", Message = "The file extension must be csv" },
                new ValidationErrorRule { RuleId = "Filename_08", Severity = "E", Message = "The date/time of the file is not greater than a previous transmission." },
                new ValidationErrorRule { RuleId = "FundingLine_01", Severity = "E", Message = "The FundingLine is not valid" },
                new ValidationErrorRule { RuleId = "FundingLine_02", Severity = "E", Message = "To claim earning adjustments against funding lines, an appropriate contract type must be held." },
                new ValidationErrorRule { RuleId = "DevolvedAreaSourceOfFunding_01", Severity = "E", Message = "The  DevolvedAreaSourceOfFunding must be returned for the FundingLine." },
                new ValidationErrorRule { RuleId = "DevolvedAreaSourceOfFunding_02", Severity = "E", Message = "The  DevolvedAreaSourceOfFunding must not be returned for the FundingLine." },
                new ValidationErrorRule { RuleId = "DevolvedAreaSourceOfFunding_03", Severity = "E", Message = "The DevolvedAreaSourceOfFunding is not a valid source of funding code." },
                new ValidationErrorRule { RuleId = "Value_01", Severity = "E", Message = "The value field must be returned" },
                new ValidationErrorRule { RuleId = "Value_03", Severity = "E", Message = "Value must be >=-99999999.99 and <=99999999.99" },
            };

            return rules;
        }
    }
}
