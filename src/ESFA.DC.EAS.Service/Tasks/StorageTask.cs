using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Common.Extensions;
using ESFA.DC.EAS.Common.Helpers;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Constants;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.Interface.Validation;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS.Service.Tasks
{
    public class StorageTask : IEasServiceTask
    {
        private readonly IEasSubmissionService _easSubmissionService;
        private readonly IEasPaymentService _easPaymentService;
        private readonly IValidationErrorLoggerService _validationErrorLoggerService;
        private readonly IFileDataCacheService _fileDataCacheService;
        private readonly ILogger _logger;

        public StorageTask(
            IEasSubmissionService easSubmissionService,
            IEasPaymentService easPaymentService,
            IValidationErrorLoggerService validationErrorLoggerService,
            IFileDataCacheService fileDataCacheService,
            ILogger logger)
        {
            _easSubmissionService = easSubmissionService;
            _easPaymentService = easPaymentService;
            _validationErrorLoggerService = validationErrorLoggerService;
            _fileDataCacheService = fileDataCacheService;
            _logger = logger;
        }

        public string TaskName => TaskNameConstants.StorageTaskName;

        public async Task ExecuteAsync(IEasJobContext easJobContext, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Storage Task is called.");

            var ukprnString = easJobContext.Ukprn.ToString();

            try
            {
                var fileDataCache = await _fileDataCacheService.GetFileDataCacheAsync(ukprnString, cancellationToken);
                if (fileDataCache != null && !fileDataCache.FailedFileValidation)
                {
                    List<PaymentType> paymentTypes = await _easPaymentService.GetAllPaymentTypes(cancellationToken);
                    Guid submissionId = Guid.NewGuid();
                    List<EasSubmission> submissionList = BuildSubmissionList(ukprnString, fileDataCache.ValidEasCsvRecords, submissionId);
                    List<EasSubmissionValue> submissionValuesList = BuildEasSubmissionValues(fileDataCache.ValidEasCsvRecords, paymentTypes, submissionId);
                    await _easSubmissionService.PersistEasSubmissionAsync(submissionList, submissionValuesList, ukprnString, cancellationToken);
                    await _validationErrorLoggerService.LogValidationErrorsAsync(easJobContext, fileDataCache.ValidationErrors, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Storage Task failed", ex);
                throw;
            }
        }

        private static List<EasSubmission> BuildSubmissionList(string ukprn, IEnumerable<EasCsvRecord> easCsvRecords, Guid submissionId)
        {
            List<int> distinctCollectionPeriods = new List<int>();
            var submissionList = new List<EasSubmission>();
            foreach (var record in easCsvRecords)
            {
                distinctCollectionPeriods.Add(
                    CollectionPeriodHelper.GetCollectionPeriod(Convert.ToInt32(record.CalendarYear), Convert.ToInt32(record.CalendarMonth)));
            }

            distinctCollectionPeriods = distinctCollectionPeriods.Distinct().ToList();

            foreach (var collectionPeriod in distinctCollectionPeriods)
            {
                var easSubmission = new EasSubmission()
                {
                    SubmissionId = submissionId,
                    CollectionPeriod = collectionPeriod,
                    DeclarationChecked = true,
                    NilReturn = false,
                    ProviderName = string.Empty,
                    Ukprn = ukprn,
                    UpdatedOn = DateTime.Now,
                };
                submissionList.Add(easSubmission);
            }

            return submissionList;
        }

        private static List<EasSubmissionValue> BuildEasSubmissionValues(IEnumerable<EasCsvRecord> easCsvRecords, List<PaymentType> paymentTypes, Guid submissionId)
        {
            var submissionValuesList = new List<EasSubmissionValue>();
            foreach (var easRecord in easCsvRecords)
            {
                var paymentType = paymentTypes.FirstOrDefault(x => x.FundingLine?.Name.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower() == easRecord.FundingLine.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower()
                                                                   && x.AdjustmentType?.Name.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower() == easRecord.AdjustmentType.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower());

                if (paymentType is null)
                {
                    throw new Exception(
                        $"Funding Line : {easRecord.FundingLine} , AdjustmentType combination :  {easRecord.AdjustmentType}  does not exist.");
                }

                var easSubmissionValues = new EasSubmissionValue
                {
                    PaymentId = paymentType.PaymentId,
                    CollectionPeriod =
                        CollectionPeriodHelper.GetCollectionPeriod(Convert.ToInt32(easRecord.CalendarYear), Convert.ToInt32(easRecord.CalendarMonth)),
                    PaymentValue = decimal.Parse(easRecord.Value),
                    DevolvedAreaSoF = string.IsNullOrEmpty(easRecord.DevolvedAreaSourceOfFunding)? -1: int.Parse(easRecord.DevolvedAreaSourceOfFunding),
                    SubmissionId = submissionId,
                };
                submissionValuesList.Add(easSubmissionValues);
            }

            return submissionValuesList;
        }
    }
}
