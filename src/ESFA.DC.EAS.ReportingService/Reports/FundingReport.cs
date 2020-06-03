using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Constants;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.Service.Mapper;

namespace ESFA.DC.EAS.ReportingService.Reports
{
    public class FundingReport : IModelReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly ICsvFileService _csvService;

        public FundingReport(IFileNameService fileNameService, ICsvFileService csvService)
        {
            _fileNameService = fileNameService;
            _csvService = csvService;
        }

        public async Task<IEnumerable<string>> GenerateReportAsync(
            IEasJobContext easContext,
            IEnumerable<EasCsvRecord> data,
            IEnumerable<ValidationErrorModel> validationErrors,
            CancellationToken cancellationToken)
        {
            var models = GetReportData(data, validationErrors);
            var fileName = _fileNameService.GetFilename(easContext.Ukprn.ToString(), easContext.JobId, ReportNameConstants.FundingReport, easContext.SubmissionDateTimeUtc, OutputTypes.Csv);
            
            await _csvService.WriteAsync<EasCsvRecord, EasCsvRecordMapper>(models, fileName, easContext.Container, cancellationToken);

            return new[] { fileName };
        }

        private List<EasCsvRecord> GetReportData(IEnumerable<EasCsvRecord> data, IEnumerable<ValidationErrorModel> validationErrors)
        {
            return data.Where(model => !validationErrors.Any(e => e.FundingLine == model.FundingLine
                                                                               && e.AdjustmentType == model.AdjustmentType
                                                                               && e.CalendarYear == model.CalendarYear
                                                                               && e.CalendarMonth == model.CalendarMonth
                                                                               && e.DevolvedAreaSoF == model.DevolvedAreaSourceOfFunding
                                                                               && e.Severity == SeverityConstants.Error)).ToList();
        }
    }
}
