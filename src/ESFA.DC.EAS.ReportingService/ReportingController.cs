using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Model;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS.ReportingService
{
    public class ReportingController : IReportingController
    {
        private readonly IStreamableKeyValuePersistenceService _streamableKeyValuePersistenceService;
        private readonly ILogger _logger;
        private readonly IValidationResultReport _resultReport;
        private readonly IList<IValidationReport> _validationReports;
        private readonly IList<IModelReport> _easReports;

        public ReportingController(
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
            ILogger logger,
            IValidationResultReport resultReport,
            IList<IValidationReport> validationReports,
            IList<IModelReport> easReports)
        {
            _streamableKeyValuePersistenceService = streamableKeyValuePersistenceService;
            _logger = logger;
            _resultReport = resultReport;
            _validationReports = validationReports;
            _easReports = easReports;
        }

        public async Task FileLevelErrorReportAsync(
            IList<EasCsvRecord> models,
            EasFileInfo fileInfo,
            IList<ValidationErrorModel> errors,
            CancellationToken cancellationToken)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                   foreach (var validationReport in _validationReports)
                    {
                        await validationReport.GenerateReportAsync(models, fileInfo, errors, archive, cancellationToken);
                    }

                    await _resultReport.GenerateReportAsync(models, fileInfo, errors, archive, cancellationToken);
                }

                await _streamableKeyValuePersistenceService.SaveAsync(
                    $"{fileInfo.UKPRN}_{fileInfo.JobId}_Reports.zip", memoryStream, cancellationToken);
            }
        }

        public async Task ProduceReportsAsync(
            IJobContextMessage jobContextMessage,
            IList<EasCsvRecord> models,
            IList<ValidationErrorModel> errors,
            EasFileInfo fileInfo,
            CancellationToken cancellationToken)
        {
            _logger.LogInfo("EAS Reporting service called");

            var reportOutputFilenamesContext = jobContextMessage.KeyValuePairs["ReportOutputFileNames"].ToString();
            var reportOutputFilenames = new List<string>();

            if (!string.IsNullOrWhiteSpace(reportOutputFilenamesContext))
            {
                reportOutputFilenames.AddRange(reportOutputFilenamesContext.Split('|').ToList());
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    foreach (var validationReport in _validationReports)
                    {
                        var reportsGenerated = await validationReport.GenerateReportAsync(models, fileInfo, errors, archive, cancellationToken);
                        reportOutputFilenames.AddRange(reportsGenerated);
                    }

                    foreach (var report in _easReports)
                    {
                        var reportsGenerated = await report.GenerateReportAsync(models, fileInfo, errors, archive, cancellationToken);
                        reportOutputFilenames.AddRange(reportsGenerated);
                    }

                    await _resultReport.GenerateReportAsync(models, fileInfo, errors, null, cancellationToken);
                }

                await _streamableKeyValuePersistenceService.SaveAsync(
                    $"{fileInfo.UKPRN}_{fileInfo.JobId}_Reports.zip", memoryStream, cancellationToken);

                jobContextMessage.KeyValuePairs["ReportOutputFileNames"] = string.Join("|", reportOutputFilenames);
            }
        }
    }
}
