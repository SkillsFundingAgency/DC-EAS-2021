using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Interface.Reports;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;

namespace ESFA.DC.EAS1819.ReportingService
{
    public class ReportingController : IReportingController
    {
        private readonly IStreamableKeyValuePersistenceService _streamableKeyValuePersistenceService;
        private readonly ILogger _logger;
        private readonly IValidationResultReport _resultReport;
        private readonly IList<IValidationReport> _validationReports;
        private readonly IList<IModelReport> _easReports;

        public ReportingController(
            [KeyFilter(PersistenceStorageKeys.AzureStorage)] IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService,
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


        public async Task FileLevelErrorReport(
            IList<EasCsvRecord> models,
            EasFileInfo sourceFile,
            IList<ValidationErrorModel> errors,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await _resultReport.GenerateReport(models, sourceFile, errors, null, cancellationToken);
        }
        public async Task ProduceReports(
            IList<EasCsvRecord> models,
            IList<ValidationErrorModel> errors,
            EasFileInfo sourceFile,
            CancellationToken cancellationToken)
        {
            _logger.LogInfo("EAS Reporting service called");

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
                        await validationReport.GenerateReport(models, sourceFile, errors, archive, cancellationToken);
                    }

                    foreach (var report in _easReports)
                    {
                        await report.GenerateReport(models, sourceFile, errors, archive, cancellationToken);
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    await _streamableKeyValuePersistenceService.SaveAsync(
                        $"{sourceFile.UKPRN}_{sourceFile.JobId}_Reports.zip", memoryStream, cancellationToken);
                }
            }
        }
    }
}
