using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Common.Helpers;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Constants;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS2021.EF;
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

        public string TaskName => TaskNameConstants.ReportingTaskName;

        public async Task ExecuteAsync(IEasJobContext easJobContext, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Reporting Task is called.");
            try
            {
                IEnumerable<ValidationErrorModel> validationErrorModels;
                IEnumerable<EasCsvRecord> easCsvRecords;

                IFileDataCache fileDataCache = await _fileDataCacheService.GetFileDataCacheAsync(easJobContext.Ukprn, cancellationToken);

                if (fileDataCache == null)
                {
                    List<PaymentType> allPaymentTypes = await _easPaymentService.GetAllPaymentTypes(cancellationToken);
                    List<EasSubmissionValue> easSubmissionValues = await _easSubmissionService.GetEasSubmissionValuesAsync(easJobContext.Ukprn, cancellationToken);
                    List<ValidationError> validationErrors = await _validationErrorService.GetValidationErrorsAsync(easJobContext.Ukprn, cancellationToken);
                    easCsvRecords = BuildEasCsvRecords(allPaymentTypes, easSubmissionValues);
                    validationErrorModels = BuildValidationErrorModels(validationErrors);
                    if (easCsvRecords.Any() || validationErrorModels.Any())
                    {
                        await _reportingController.ProduceReportsAsync(easJobContext, easCsvRecords, validationErrorModels, cancellationToken);
                    }
                }

                if (fileDataCache != null && !fileDataCache.FailedFileValidation)
                {
                    easCsvRecords = fileDataCache.AllEasCsvRecords;
                    validationErrorModels = fileDataCache.ValidationErrors;
                    await _reportingController.ProduceReportsAsync(easJobContext, easCsvRecords, validationErrorModels, cancellationToken);
                }

                if (fileDataCache != null && fileDataCache.FailedFileValidation)
                {
                    _logger.LogError($"Reports are not generated as File- {easJobContext.FileReference} failed file Validation");
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Reporting Task failed", ex);
                throw;
            }
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
                    RuleName = error.RuleId,
                    DevolvedAreaSoF = error.DevolvedAreaSoF
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
                    CalendarMonth = CollectionPeriodHelper.GetCalendarYearAndMonth(submissionValue.CollectionPeriod).Item2.ToString(),
                    DevolvedAreaSourceOfFunding = submissionValue.DevolvedAreaSoF == -1? null : submissionValue.DevolvedAreaSoF.ToString()
                };
                records.Add(record);
            }

            return records;
        }
    }
}
