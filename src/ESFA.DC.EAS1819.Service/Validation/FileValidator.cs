namespace ESFA.DC.EAS1819.Service.Validation
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CsvHelper;
    using FluentValidation;

    public class FileValidator : AbstractValidator<IList<string>>
    {
        static string[] EasCSVHeaders =
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
            if (!easCSVHeaders.SequenceEqual<string>(EasCSVHeaders))
            {
                return false;
            }

            return true;
        }
    }
}
