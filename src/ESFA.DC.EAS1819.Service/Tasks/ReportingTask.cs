using System.Globalization;
using ESFA.DC.EAS1819.Common.Helpers;

namespace ESFA.DC.EAS1819.Service.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ESFA.DC.EAS1819.DataService.Interface;
    using ESFA.DC.EAS1819.EF;
    using ESFA.DC.EAS1819.Interface;
    using ESFA.DC.EAS1819.Interface.FileData;
    using ESFA.DC.EAS1819.Interface.Reports;
    using ESFA.DC.EAS1819.Model;
    using ESFA.DC.EAS1819.Service.Helpers;
    using ESFA.DC.JobContext.Interface;
    using ESFA.DC.JobContextManager.Model.Interface;
    using ESFA.DC.Logging.Interfaces;

    public class ReportingTask : IEasServiceTask
    {
        private readonly IEasSubmissionService _easSubmissionService;
        private readonly IValidationErrorService _validationErrorService;
        private readonly IEasPaymentService _easPaymentService;
        private readonly IFileDataCacheService _fileDataCacheService;
        private readonly IReportingController _reportingController;
        private readonly ILogger _logger;

        public ReportingTask(
            IEasSubmissionService easSubmissionService,
            IValidationErrorService validationErrorService,
            IEasPaymentService easPaymentService,
            IFileDataCacheService fileDataCacheService,
            IReportingController reportingController,
            ILogger logger)
        {
            _easSubmissionService = easSubmissionService;
            _validationErrorService = validationErrorService;
            _easPaymentService = easPaymentService;
            _fileDataCacheService = fileDataCacheService;
            _reportingController = reportingController;
            _logger = logger;
        }

        public string TaskName => "Reporting";

        public async Task ExecuteAsync(IJobContextMessage jobContextMessage, EasFileInfo fileInfo, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Reporting Task is called.");
            try
            {
                List<ValidationErrorModel> validationErrorModels;
                List<EasCsvRecord> easCsvRecords;
                EasFileInfo easfileInfo;
                var ukPrn = jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString();
                var fileDataCache = await _fileDataCacheService.GetFileDataCacheAsync(ukPrn, cancellationToken);
                easfileInfo = BuildEasFileInfo(jobContextMessage);

                if (fileDataCache == null)
                {
                    var allPaymentTypes = _easPaymentService.GetAllPaymentTypes();
                    var easSubmissionValues = await _easSubmissionService.GetEasSubmissionValuesAsync(ukPrn);
                    var validationErrors = await _validationErrorService.GetValidationErrorsAsync(ukPrn);
                    easCsvRecords = BuildEasCsvRecords(allPaymentTypes, easSubmissionValues);
                    validationErrorModels = BuildValidationErrorModels(validationErrors);
                    if (easCsvRecords.Any() || validationErrorModels.Any())
                    {
                        await _reportingController.ProduceReportsAsync(easCsvRecords, validationErrorModels, easfileInfo, cancellationToken);
                    }
                }

                if (fileDataCache != null && !fileDataCache.FailedFileValidation)
                {
                    easCsvRecords = fileDataCache.AllEasCsvRecords;
                    validationErrorModels = fileDataCache.ValidationErrors;
                    await _reportingController.ProduceReportsAsync(easCsvRecords, validationErrorModels, easfileInfo, cancellationToken);
                }

                if (fileDataCache != null && fileDataCache.FailedFileValidation)
                {
                    _logger.LogError($"Reports are not generated as File- {easfileInfo.FileName} failed file Validation");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Reporting Task failed", ex);
                throw;
            }
        }

        private EasFileInfo BuildEasFileInfo(IJobContextMessage jobContextMessage)
        {
            EasFileInfo easFileInfo = new EasFileInfo
            {
                UKPRN = jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString(),
                DateTime = jobContextMessage.SubmissionDateTimeUtc,
                JobId = jobContextMessage.JobId
            };
            return easFileInfo;
        }

        private List<ValidationErrorModel> BuildValidationErrorModels(List<ValidationError> validationErrors)
        {
            List<ValidationErrorModel> validationErrorModels = new List<ValidationErrorModel>();
            foreach (var error in validationErrors)
            {
                var model = new ValidationErrorModel()
                {
                    AdjustmentType = error.AdjustmentType,
                    FundingLine = error.FundingLine,
                    Value = error.Value,
                    CalendarYear = error.CalendarYear,
                    ErrorMessage = error.ErrorMessage,
                    CalendarMonth = error.CalendarMonth,
                    Severity = error.Severity,
                    RuleName = error.RuleId
                };
                validationErrorModels.Add(model);
            }

            return validationErrorModels;
        }

        private List<EasCsvRecord> BuildEasCsvRecords(List<PaymentTypes> allPaymentTypes, List<EasSubmissionValues> easSubmissionValues)
        {
            List<EasCsvRecord> records = new List<EasCsvRecord>();
            foreach (var submissionValue in easSubmissionValues)
            {
                var paymentType = allPaymentTypes.FirstOrDefault(x => x.PaymentId == submissionValue.PaymentId);
                var record = new EasCsvRecord()
                {
                    AdjustmentType = paymentType.AdjustmentType.Name,
                    FundingLine = paymentType.FundingLine.Name,
                    Value = submissionValue.PaymentValue.ToString(CultureInfo.InvariantCulture),
                    CalendarYear = CollectionPeriodHelper.GetCalendarYearAndMonth(submissionValue.CollectionPeriod).Item1.ToString(),
                    CalendarMonth = CollectionPeriodHelper.GetCalendarYearAndMonth(submissionValue.CollectionPeriod).Item2.ToString()
                };
                records.Add(record);
            }

            return records;
        }
    }
}
