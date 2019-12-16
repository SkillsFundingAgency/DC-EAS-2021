using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.Service.Mapper;
using ESFA.DC.IO.Interfaces;

namespace ESFA.DC.EAS.ReportingService.Reports
{
    public class FundingReport : AbstractReportBuilder, IModelReport
    {
        private readonly IStreamableKeyValuePersistenceService _streamableKeyValuePersistenceService;
        private readonly IFileNameService _fileNameService;

        public FundingReport(
            IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService, 
            IFileNameService fileNameService)
        {
            _streamableKeyValuePersistenceService = streamableKeyValuePersistenceService;
            _fileNameService = fileNameService;
            ReportFileName = "EAS Funding Report";
        }

        public async Task<IEnumerable<string>> GenerateReportAsync(
            IList<EasCsvRecord> data,
            EasFileInfo fileInfo,
            IList<ValidationErrorModel> validationErrors,
            ZipArchive archive,
            CancellationToken cancellationToken)
        {
            var csv = GetCsv(data, validationErrors);

            var externalFileName = _fileNameService.GetExternalFilename(fileInfo.UKPRN, fileInfo.JobId, ReportFileName, fileInfo.DateTime, OutputTypes.Csv);
            var fileName = _fileNameService.GetFilename(ReportFileName, fileInfo.DateTime, OutputTypes.Csv);
            var reportFileName = externalFileName.Replace('_', '/');

            await _streamableKeyValuePersistenceService.SaveAsync(externalFileName, csv, cancellationToken);
            await WriteZipEntry(archive, fileName, csv);
            return new[] { reportFileName };
        }

        private string GetCsv(IList<EasCsvRecord> data, IList<ValidationErrorModel> validationErrors)
        {
            var easCsvRecords = data.Where(model => !validationErrors.Any(e => e.FundingLine == model.FundingLine
                                                                               && e.AdjustmentType == model.AdjustmentType
                                                                               && e.CalendarYear == model.CalendarYear
                                                                               && e.CalendarMonth == model.CalendarMonth
                                                                               && e.DevolvedAreaSoF == model.DevolvedAreaSourceOfFunding
                                                                               && e.Severity == "E")).ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                BuildCsvReport<EasCsvRecordMapper, EasCsvRecord>(ms, easCsvRecords);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
