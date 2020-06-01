using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Validation;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS2021.EF;

namespace ESFA.DC.EAS.ValidationService
{
    public class ValidationErrorLoggerService : IValidationErrorLoggerService
    {
        private readonly IValidationErrorService _validationErrorService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ValidationErrorLoggerService(IValidationErrorService validationErrorService, IDateTimeProvider dateTimeProvider)
        {
            _validationErrorService = validationErrorService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task LogValidationErrorsAsync(IEasJobContext easJobContext, IEnumerable<ValidationErrorModel> validationErrors, CancellationToken cancellationToken)
        {
            var validationErrorList = new List<ValidationError>();
            var sourceFile = new SourceFile()
            {
                Ukprn = easJobContext.Ukprn.ToString(),
                DateTime = easJobContext.SubmissionDateTimeUtc,
                FileName = easJobContext.FileReference,
                FilePreparationDate = BuildFilePrepDate(easJobContext.FileReference)
            };

            int sourceFileId = await _validationErrorService.LogErrorSourceFileAsync(sourceFile, cancellationToken);

            foreach (var error in validationErrors)
            {
                var validationError = new ValidationError()
                {
                    AdjustmentType = error.AdjustmentType,
                    Value = error.Value,
                    CalendarMonth = error.CalendarMonth,
                    CalendarYear = error.CalendarYear,
                    CreatedOn = DateTime.UtcNow,
                    ErrorMessage = error.ErrorMessage,
                    FundingLine = error.FundingLine,
                    RowId = Guid.NewGuid(), //TODO: find out if this is right.
                    RuleId = error.RuleName,
                    Severity = error.Severity,
                    SourceFileId = sourceFileId,
                    DevolvedAreaSoF = error.DevolvedAreaSoF
                };

                validationErrorList.Add(validationError);
            }

            await _validationErrorService.LogValidationErrorsAsync(validationErrorList, cancellationToken);
        }

        private DateTime BuildFilePrepDate(string filename)
        {
            var fileNameParts = filename.Substring(0, filename.IndexOf('.')).Split('-');
            if (fileNameParts.Length != 4)
            {
                throw new ArgumentException($"Filename is invalid");
            }

            var ukprn = fileNameParts[1];
            var datePart = fileNameParts[2];
            var timePart = fileNameParts[3];

            return _dateTimeProvider.ConvertUkToUtc(string.Format($"{datePart}-{timePart}"));
        }
    }
}
