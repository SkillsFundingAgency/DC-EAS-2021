using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Constants;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ReportingService.Mapper;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.EAS.ReportingService.Reports
{
    public class ViolationReport : IValidationReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly ICsvService _csvService;

        public ViolationReport( 
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
            var models = GetReportData(validationErrors);
            var fileName = _fileNameService.GetFilename(fileInfo.UKPRN, fileInfo.JobId, ReportNameConstants.ViolationReport, fileInfo.DateTime, OutputTypes.Csv);

            await _csvService.WriteAsync<ValidationErrorModel, EasCsvViolationRecordMapper>(models, fileName, jobContextMessage.KeyValuePairs[JobContextMessageKey.Container].ToString(), cancellationToken);

            return new[] { fileName };
        }

        private IOrderedEnumerable<ValidationErrorModel> GetReportData(IList<ValidationErrorModel> validationErrors)
        {
            return validationErrors.OrderBy(x => x.Severity).ThenBy(x => x.RuleName);
        }
    }
}
