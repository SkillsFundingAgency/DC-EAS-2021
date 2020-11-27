using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Model;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS.ReportingService
{
    public class ReportingController : IReportingController
    {
        private const string _zipName = "Reports";

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
            IEasJobContext easContext,
            IEnumerable<EasCsvRecord> models,
            IEnumerable<ValidationErrorModel> errors,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var reportOutputFilenames = new List<string>();

            if (!string.IsNullOrWhiteSpace(easContext.ReportOutputFileNames))
            {
                reportOutputFilenames.AddRange(easContext.ReportOutputFileNames.Split('|'));
            }

            foreach (var validationReport in _validationReports)
            {
                var reportsGenerated = await validationReport.GenerateReportAsync(easContext, models, errors, cancellationToken);
                reportOutputFilenames.AddRange(reportsGenerated);
            }

            await _resultReport.GenerateReportAsync(easContext, models, errors, cancellationToken);

            var zipName = _fileNameService.GetZipName(easContext.Ukprn, easContext.JobId, _zipName);

            await _zipService.CreateZipAsync(zipName, reportOutputFilenames, easContext.Container.ToString(), cancellationToken);

            easContext.ReportOutputFileNames = string.Join("|", reportOutputFilenames);
        }

        public async Task ProduceReportsAsync(
            IEasJobContext easContext,
            IEnumerable<EasCsvRecord> models,
            IEnumerable<ValidationErrorModel> errors,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            _logger.LogInfo("EAS Reporting service called");

            var reportOutputFilenames = new List<string>();

            if (!string.IsNullOrWhiteSpace(easContext.ReportOutputFileNames))
            {
                reportOutputFilenames.AddRange(easContext.ReportOutputFileNames.Split('|'));
            }


            foreach (var validationReport in _validationReports)
            {
                var reportsGenerated = await validationReport.GenerateReportAsync(easContext, models, errors, cancellationToken);
                reportOutputFilenames.AddRange(reportsGenerated);
            }

            foreach (var report in _easReports)
            {
                var reportsGenerated = await report.GenerateReportAsync(easContext, models, errors, cancellationToken);
                reportOutputFilenames.AddRange(reportsGenerated);
            }

            await _resultReport.GenerateReportAsync(easContext, models, errors, cancellationToken);

            var zipName = _fileNameService.GetZipName(easContext.Ukprn, easContext.JobId, _zipName);

            await _zipService.CreateZipAsync(zipName, reportOutputFilenames, easContext.Container.ToString(), cancellationToken);

            easContext.ReportOutputFileNames = string.Join("|", reportOutputFilenames);
        }
    }
}
