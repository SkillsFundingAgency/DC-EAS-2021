using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Common.Helpers;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS1920.EF;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS.Service.Tasks
{
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
                string ukPrn = jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString();
                IFileDataCache fileDataCache = await _fileDataCacheService.GetFileDataCacheAsync(ukPrn, cancellationToken);
                easfileInfo = BuildEasFileInfo(jobContextMessage);

                if (fileDataCache == null)
                {
                    List<PaymentType> allPaymentTypes = await _easPaymentService.GetAllPaymentTypes(cancellationToken);
                    List<EasSubmissionValue> easSubmissionValues = await _easSubmissionService.GetEasSubmissionValuesAsync(ukPrn, cancellationToken);
                    List<ValidationError> validationErrors = await _validationErrorService.GetValidationErrorsAsync(ukPrn, cancellationToken);
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
                var model = new ValidationErrorModel
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

        private List<EasCsvRecord> BuildEasCsvRecords(List<PaymentType> allPaymentTypes, List<EasSubmissionValue> easSubmissionValues)
        {
            List<EasCsvRecord> records = new List<EasCsvRecord>();
            foreach (var submissionValue in easSubmissionValues)
            {
                var paymentType = allPaymentTypes.FirstOrDefault(x => x.PaymentId == submissionValue.PaymentId);
                var record = new EasCsvRecord
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
