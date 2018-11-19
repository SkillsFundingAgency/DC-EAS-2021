using ESFA.DC.EAS1819.Model;
using FluentValidation;
using FluentValidation.Results;

namespace ESFA.DC.EAS1819.ValidationService.Extensions
{
    public static class EasRecordExtensions
    {
        public static ValidationErrorModel ToValidationErrorModel(this EasCsvRecord record, ValidationFailure error)
        {
            var errorModel = new ValidationErrorModel()
            {
                FundingLine = record.FundingLine,
                AdjustmentType = record.AdjustmentType,
                CalendarYear = record.CalendarYear,
                CalendarMonth = record.CalendarMonth,
                Value = record.Value.GetValueOrDefault(),
                ErrorMessage = error.ErrorMessage,
                RuleName = error.ErrorCode,
                Severity = error.Severity == Severity.Warning ? "W" : "E"
            };
            return errorModel;
        }
    }
}
