﻿using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ReportingService.Mapper;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.EAS.ReportingService.Reports
{
    public class ValidationResultReport : AbstractReportBuilder, IValidationResultReport
    {
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IStreamableKeyValuePersistenceService _streamableKeyValuePersistenceService;

        public ValidationResultReport(
            IJsonSerializationService jsonSerializationService,
            IDateTimeProvider dateTimeProvider,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService)
            : base(dateTimeProvider)
        {
            _jsonSerializationService = jsonSerializationService;
            _streamableKeyValuePersistenceService = streamableKeyValuePersistenceService;
            ReportFileName = "EAS Validation Result Report";
        }

        public async Task<IEnumerable<string>> GenerateReportAsync(
            IList<EasCsvRecord> data,
            EasFileInfo fileInfo,
            IList<ValidationErrorModel> validationErrors,
            ZipArchive archive,
            CancellationToken cancellationToken)
        {
            var report = GetValidationReport(data, validationErrors);

            var externalFileName = GetExternalFilename(fileInfo.UKPRN, fileInfo.JobId, fileInfo.DateTime);
            var reportFileName = externalFileName.Replace('_', '/');

            await SaveJson(externalFileName, report, cancellationToken);
            return new[] { $"{reportFileName}.json" };
        }

        private FileValidationResult GetValidationReport(
            IList<EasCsvRecord> data,
            IList<ValidationErrorModel> validationErrors)
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

        private async Task SaveJson(string fileName, FileValidationResult result, CancellationToken cancellationToken)
        {
            await _streamableKeyValuePersistenceService.SaveAsync($"{fileName}.json", _jsonSerializationService.Serialize(result), cancellationToken);
        }

        private string GetCsv(IList<ValidationErrorModel> validationErrors)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BuildCsvReport<EasCsvViolationRecordMapper, ValidationErrorModel>(ms, validationErrors);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
