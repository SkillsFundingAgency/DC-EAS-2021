namespace ESFA.DC.EAS.ValidationService.Validators
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;

    public class FileValidator : AbstractValidator<IList<string>>
    {
        private static string[] _easCSVHeaders =
        {
            "FundingLine", "AdjustmentType", "CalendarYear", "CalendarMonth", "Value"
        };

        public FileValidator()
        {
            RuleFor(x => x)
                .Must(HaveValidHeaders)
                .WithMessage("The file format is incorrect.  Please check the field headers are as per the Guidance document.")
                .WithErrorCode("Fileformat_01")
                .WithSeverity(Severity.Error);
        }

        private bool HaveValidHeaders(IList<string> easCSVHeaders)
        {
            if (!easCSVHeaders.SequenceEqual<string>(_easCSVHeaders))
            {
                return false;
            }

            return true;
        }
    }
}
