using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Constants;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Model;
using ESFA.DC.FileService.Interface;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
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
            IJobContextMessage jobContextMessage,
            IList<EasCsvRecord> data,
            EasFileInfo fileInfo,
            IList<ValidationErrorModel> validationErrors,
            CancellationToken cancellationToken)
        {
            var report = GetValidationReport(data, validationErrors);
            var fileName = _fileNameService.GetFilename(fileInfo.UKPRN, fileInfo.JobId, ReportNameConstants.ValidationResultReport, fileInfo.DateTime, OutputTypes.Json);

            using (var fileStream = await _fileService.OpenWriteStreamAsync(fileName, jobContextMessage.KeyValuePairs[JobContextMessageKey.Container].ToString(), cancellationToken))
            {
                _jsonSerializationService.Serialize(report, fileStream);
            }

            return new[] { fileName };
        }

        private FileValidationResult GetValidationReport(IList<EasCsvRecord> data, IList<ValidationErrorModel> validationErrors)
        {
            var errors = validationErrors.Where(x => x.Severity == "E").ToList();
            var warnings = validationErrors.Where(x => x.Severity == "W").ToList();

            return new FileValidationResult
            {
                TotalLearners = data?.Count ?? 0,
                TotalErrors = errors.Count,
                TotalWarnings = warnings.Count,
                TotalWarningLearners = warnings.Count,
                TotalErrorLearners = errors.Count,
                ErrorMessage = validationErrors.FirstOrDefault(x => string.IsNullOrEmpty(x.FundingLine))?.ErrorMessage
            };
        }
    }
}
