using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Model;
using FluentValidation;
using FluentValidation.Results;

namespace ESFA.DC.EAS1819.ValidationService.Extensions
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
                Severity = validationErrorRule.Severity
            };
            return errorModel;
        }
    }
}
