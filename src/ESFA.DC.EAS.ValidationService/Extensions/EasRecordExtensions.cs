using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS1920.EF;
using FluentValidation.Results;

namespace ESFA.DC.EAS.ValidationService.Extensions
{
    public static class EasRecordExtensions
    {
        public static ValidationErrorModel ToValidationErrorModel(this EasCsvRecord record, ValidationFailure error, List<ValidationErrorRule> validationErrorRules)
        {
            var validationErrorRule = validationErrorRules.FirstOrDefault(x => x.RuleId == error.ErrorCode);
            if (validationErrorRule == null)
            {
                throw new Exception($"ValidationErrorRule Not found for the error code:  {error.ErrorCode}");
            }

            var errorModel = new ValidationErrorModel()
            {
                FundingLine = record.FundingLine,
                AdjustmentType = record.AdjustmentType,
                CalendarYear = record.CalendarYear,
                CalendarMonth = record.CalendarMonth,
                Value = record.Value,
                ErrorMessage = validationErrorRule.Message,
                RuleName = error.ErrorCode,
                Severity = validationErrorRule.Severity,
                DevolvedAreaSoF = record.DevolvedAreaSourceOfFunding
            };
            return errorModel;
        }
    }
}
