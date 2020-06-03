using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Constants;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Model;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.EAS.ReportingService.Reports
{
    public class ValidationResultReport : IValidationResultReport
    {
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IFileNameService _fileNameService;
        private readonly IFileService _fileService;

        public ValidationResultReport(
            IJsonSerializationService jsonSerializationService, 
            IFileNameService fileNameService, 
            IFileService fileService)
        {
            _jsonSerializationService = jsonSerializationService;
            _fileNameService = fileNameService;
            _fileService = fileService;
        }

        public async Task<IEnumerable<string>> GenerateReportAsync(
            IEasJobContext easContext,
            IEnumerable<EasCsvRecord> data,
            IEnumerable<ValidationErrorModel> validationErrors,
            CancellationToken cancellationToken)
        {
            var report = GetValidationReport(data, validationErrors);
            var fileName = _fileNameService.GetFilename(easContext.Ukprn, easContext.JobId, ReportNameConstants.ValidationResultReport, easContext.SubmissionDateTimeUtc, OutputTypes.Json);

            using (var fileStream = await _fileService.OpenWriteStreamAsync(fileName, easContext.Container, cancellationToken))
            {
                _jsonSerializationService.Serialize(report, fileStream);
            }

            return new[] { fileName };
        }

        private FileValidationResult GetValidationReport(IEnumerable<EasCsvRecord> data, IEnumerable<ValidationErrorModel> validationErrors)
        {
            var errors = validationErrors.Where(x => x.Severity == SeverityConstants.Error).ToList();
            var warnings = validationErrors.Where(x => x.Severity == SeverityConstants.Warning).ToList();

            return new FileValidationResult
            {
                TotalLearners = data?.Count() ?? 0,
                TotalErrors = errors.Count,
                TotalWarnings = warnings.Count,
                TotalWarningLearners = warnings.Count,
                TotalErrorLearners = errors.Count,
                ErrorMessage = validationErrors.FirstOrDefault(x => string.IsNullOrEmpty(x.FundingLine))?.ErrorMessage
            };
        }
    }
}
