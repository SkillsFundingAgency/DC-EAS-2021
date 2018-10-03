using ESFA.DC.EAS1819.Model;
using FluentValidation;
using FluentValidation.Results;

namespace ESFA.DC.EAS1819.Service.Extensions
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
                Value = record.Value,
                ErrorMessage = error.ErrorMessage,
                RuleName = error.ErrorCode,
                IsWarning = error.Severity == Severity.Warning
            };
            return errorModel;
        }
    }
}
