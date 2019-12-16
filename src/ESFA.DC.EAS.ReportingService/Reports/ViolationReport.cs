using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ReportingService.Mapper;
using ESFA.DC.IO.Interfaces;

namespace ESFA.DC.EAS.ReportingService.Reports
{
    public class ViolationReport : AbstractReportBuilder, IValidationReport
    {
        private readonly IStreamableKeyValuePersistenceService _streamableKeyValuePersistenceService;
        private readonly IFileNameService _fileNameService;

        public ViolationReport(
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService, 
            IFileNameService fileNameService)
        {
            _streamableKeyValuePersistenceService = streamableKeyValuePersistenceService;
            _fileNameService = fileNameService;
            ReportFileName = "EAS Rule Violation Report";
        }

        public async Task<IEnumerable<string>> GenerateReportAsync(
            IList<EasCsvRecord> data,
            EasFileInfo fileInfo,
            IList<ValidationErrorModel> validationErrors,
            ZipArchive archive,
            CancellationToken cancellationToken)
        {
            var csv = GetCsv(validationErrors);

            var externalFileName = _fileNameService.GetExternalFilename(fileInfo.UKPRN, fileInfo.JobId, ReportFileName, fileInfo.DateTime, OutputTypes.Csv);
            var fileName = _fileNameService.GetFilename(ReportFileName, fileInfo.DateTime, OutputTypes.Csv);
            var reportFileName = externalFileName.Replace('_', '/');

            await _streamableKeyValuePersistenceService.SaveAsync(externalFileName, csv, cancellationToken);
            await WriteZipEntry(archive, fileName, csv);
            return new[] { reportFileName };
        }

        private string GetCsv(IList<ValidationErrorModel> validationErrors)
        {
            var validationErrorModels = validationErrors.OrderBy(x => x.Severity).ThenBy(x => x.RuleName);
            using (MemoryStream ms = new MemoryStream())
            {
                BuildCsvReport<EasCsvViolationRecordMapper, ValidationErrorModel>(ms, validationErrorModels);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
