using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Model;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS.ReportingService
{
    public class ReportingController : IReportingController
    {
        private readonly ILogger _logger;
        private readonly IValidationResultReport _resultReport;
        private readonly IList<IValidationReport> _validationReports;
        private readonly IList<IModelReport> _easReports;
        private readonly IZipService _zipService;
        private readonly IFileNameService _fileNameService;

        public ReportingController(
            ILogger logger,
            IValidationResultReport resultReport,
            IList<IValidationReport> validationReports,
            IList<IModelReport> easReports, 
            IZipService zipService, 
            IFileNameService fileNameService)
        {
            _logger = logger;
            _resultReport = resultReport;
            _validationReports = validationReports;
            _easReports = easReports;
            _zipService = zipService;
            _fileNameService = fileNameService;
        }

        public async Task FileLevelErrorReportAsync(
            IJobContextMessage jobContextMessage,
            IList<EasCsvRecord> models,
            EasFileInfo fileInfo,
            IList<ValidationErrorModel> errors,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (!jobContextMessage.KeyValuePairs.ContainsKey("ReportOutputFileNames"))
            {
                jobContextMessage.KeyValuePairs.Add("ReportOutputFileNames", string.Empty);
            }

            var reportOutputFilenamesContext = jobContextMessage.KeyValuePairs["ReportOutputFileNames"].ToString();
            var reportOutputFilenames = new List<string>();

            if (!string.IsNullOrWhiteSpace(reportOutputFilenamesContext))
            {
                reportOutputFilenames.AddRange(reportOutputFilenamesContext.Split('|'));
            }

            foreach (var validationReport in _validationReports)
            {
                var reportsGenerated = await validationReport.GenerateReportAsync(jobContextMessage, models, fileInfo, errors, cancellationToken);
                reportOutputFilenames.AddRange(reportsGenerated);
            }

            await _resultReport.GenerateReportAsync(jobContextMessage, models, fileInfo, errors, cancellationToken);

            var zipName = _fileNameService.GetZipName(fileInfo.UKPRN, fileInfo.JobId, "Reports");

            await _zipService.CreateZipAsync(zipName, reportOutputFilenames, jobContextMessage.KeyValuePairs[JobContextMessageKey.Container].ToString(), cancellationToken);

            jobContextMessage.KeyValuePairs["ReportOutputFileNames"] = string.Join("|", reportOutputFilenames);
        }

        public async Task ProduceReportsAsync(
            IJobContextMessage jobContextMessage,
            IList<EasCsvRecord> models,
            IList<ValidationErrorModel> errors,
            EasFileInfo fileInfo,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            _logger.LogInfo("EAS Reporting service called");

            if (!jobContextMessage.KeyValuePairs.ContainsKey("ReportOutputFileNames"))
            {
                jobContextMessage.KeyValuePairs.Add("ReportOutputFileNames", string.Empty);
            }

            var reportOutputFilenamesContext = jobContextMessage.KeyValuePairs["ReportOutputFileNames"].ToString();
            var reportOutputFilenames = new List<string>();

            if (!string.IsNullOrWhiteSpace(reportOutputFilenamesContext))
            {
                reportOutputFilenames.AddRange(reportOutputFilenamesContext.Split('|'));
            }

            foreach (var validationReport in _validationReports)
            {
                var reportsGenerated = await validationReport.GenerateReportAsync(jobContextMessage, models, fileInfo, errors, cancellationToken);
                reportOutputFilenames.AddRange(reportsGenerated);
            }

            foreach (var report in _easReports)
            {
                var reportsGenerated = await report.GenerateReportAsync(jobContextMessage, models, fileInfo, errors, cancellationToken);
                reportOutputFilenames.AddRange(reportsGenerated);
            }

            await _resultReport.GenerateReportAsync(jobContextMessage, models, fileInfo, errors, cancellationToken);

            var zipName = _fileNameService.GetZipName(fileInfo.UKPRN, fileInfo.JobId, "Reports");

            await _zipService.CreateZipAsync(zipName, reportOutputFilenames, jobContextMessage.KeyValuePairs[JobContextMessageKey.Container].ToString(), cancellationToken);

            jobContextMessage.KeyValuePairs["ReportOutputFileNames"] = string.Join("|", reportOutputFilenames);
        }
    }
}
