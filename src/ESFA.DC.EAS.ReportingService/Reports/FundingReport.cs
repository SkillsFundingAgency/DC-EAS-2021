using System;
using System.Collections.Generic;
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
using ESFA.DC.EAS.Service.Helpers;
using ESFA.DC.EAS.Service.Mapper;
using ESFA.DC.IO.Interfaces;

namespace ESFA.DC.EAS.ReportingService.Reports
{
    public class FundingReport : AbstractReportBuilder, IModelReport
    {
        private readonly IStreamableKeyValuePersistenceService _streamableKeyValuePersistenceService;

        public FundingReport(
            IDateTimeProvider dateTimeProvider,
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService) : base(dateTimeProvider)
        {
            _streamableKeyValuePersistenceService = streamableKeyValuePersistenceService;
            ReportFileName = "EAS Funding Report";
        }

        public async Task GenerateReportAsync(
            IList<EasCsvRecord> data,
            EasFileInfo fileInfo,
            IList<ValidationErrorModel> validationErrors,
            ZipArchive archive,
            CancellationToken cancellationToken)
        {
            var csv = GetCsv(data, validationErrors);

            var externalFileName = GetExternalFilename(fileInfo.UKPRN, fileInfo.JobId, fileInfo.DateTime);
            var fileName = GetFilename(fileInfo.UKPRN, fileInfo.JobId, fileInfo.DateTime);

            await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private string GetCsv(IList<EasCsvRecord> data, IList<ValidationErrorModel> validationErrors)
        {
            var easCsvRecords = data.Where(model => !validationErrors.Any(e => e.FundingLine == model.FundingLine
                                                                               && e.AdjustmentType == model.AdjustmentType
                                                                               && e.CalendarYear == model.CalendarYear
                                                                               && e.CalendarMonth == model.CalendarMonth
                                                                               && e.Severity == "E")).ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                BuildCsvReport<EasCsvRecordMapper, EasCsvRecord>(ms, easCsvRecords);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
