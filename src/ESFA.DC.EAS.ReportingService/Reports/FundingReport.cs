using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Constants;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.Service.Mapper;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.EAS.ReportingService.Reports
{
    public class FundingReport : IModelReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly ICsvService _csvService;

        public FundingReport( 
            IFileNameService fileNameService, 
            ICsvService csvService)
        {
            _fileNameService = fileNameService;
            _csvService = csvService;
        }

        public async Task<IEnumerable<string>> GenerateReportAsync(
            IJobContextMessage jobContextMessage,
            IList<EasCsvRecord> data,
            EasFileInfo fileInfo,
            IList<ValidationErrorModel> validationErrors,
            CancellationToken cancellationToken)
        {
            var models = GetReportData(data, validationErrors);
            var fileName = _fileNameService.GetFilename(fileInfo.UKPRN, fileInfo.JobId, ReportNameConstants.FundingReport, fileInfo.DateTime, OutputTypes.Csv);
            
            await _csvService.WriteAsync<EasCsvRecord, EasCsvRecordMapper>(models, fileName, jobContextMessage.KeyValuePairs[JobContextMessageKey.Container].ToString(), cancellationToken);

            return new[] { fileName };
        }

        private List<EasCsvRecord> GetReportData(IList<EasCsvRecord> data, IList<ValidationErrorModel> validationErrors)
        {
            return data.Where(model => !validationErrors.Any(e => e.FundingLine == model.FundingLine
                                                                               && e.AdjustmentType == model.AdjustmentType
                                                                               && e.CalendarYear == model.CalendarYear
                                                                               && e.CalendarMonth == model.CalendarMonth
                                                                               && e.DevolvedAreaSoF == model.DevolvedAreaSourceOfFunding
                                                                               && e.Severity == "E")).ToList();
        }
    }
}
