using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Constants;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ReportingService.Mapper;

namespace ESFA.DC.EAS.ReportingService.Reports
{
    public class ViolationReport : IValidationReport
    {
        private readonly IFileNameService _fileNameService;
        private readonly ICsvFileService _csvService;

        public ViolationReport( 
            IFileNameService fileNameService, 
            ICsvFileService csvService)
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
            var models = GetReportData(validationErrors);
            var fileName = _fileNameService.GetFilename(easContext.Ukprn.ToString(), easContext.JobId, ReportNameConstants.ViolationReport, easContext.SubmissionDateTimeUtc, OutputTypes.Csv);

            await _csvService.WriteAsync<ValidationErrorModel, EasCsvViolationRecordMapper>(models, fileName, easContext.Container, cancellationToken);

            return new[] { fileName };
        }

        private IOrderedEnumerable<ValidationErrorModel> GetReportData(IEnumerable<ValidationErrorModel> validationErrors)
        {
            return validationErrors.OrderBy(x => x.Severity).ThenBy(x => x.RuleName);
        }
    }
}
