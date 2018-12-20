using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Common.Helpers;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Interface.Reports;
using ESFA.DC.EAS1819.Interface.Validation;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS1819.Service.Import
{
    public class ImportService : IImportService
    {
        private readonly Guid _submissionId;
        private readonly IEasSubmissionService _easSubmissionService;
        private readonly IEasPaymentService _easPaymentService;
        private readonly IValidationService _validationService;
        private readonly IReportingController _reportingController;
        private readonly ILogger _logger;

        public ImportService(
            IEasSubmissionService easSubmissionService,
            IEasPaymentService easPaymentService,
            IValidationService validationService,
            IReportingController reportingController,
            ILogger logger)
        {
            _easSubmissionService = easSubmissionService;
            _easPaymentService = easPaymentService;
            _validationService = validationService;
            _reportingController = reportingController;
            _logger = logger;
        }

        public ImportService(
            Guid submissionId,
            IEasSubmissionService easSubmissionService,
            IEasPaymentService easPaymentService,
            IValidationService validationService,
            IReportingController reportingController,
            ILogger logger)
            : this(easSubmissionService, easPaymentService, validationService, reportingController, logger)
        {
            _submissionId = submissionId;
        }

        public async Task ImportEasDataAsync(EasFileInfo fileInfo, IList<EasCsvRecord> easCsvRecords, CancellationToken cancellationToken)
        {
            List<ValidationErrorModel> validationErrorModels = await _validationService.ValidateDataAsync(fileInfo, easCsvRecords.ToList(), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            List<EasCsvRecord> validRecords = GetValidRows(easCsvRecords, validationErrorModels);
            if (validRecords.Count > 0)
            {
                List<PaymentType> paymentTypes = await _easPaymentService.GetAllPaymentTypes(cancellationToken);
                Guid submissionId = _submissionId != Guid.Empty ? _submissionId : Guid.NewGuid();
                List<EasSubmission> submissionList = BuildSubmissionList(fileInfo, validRecords, submissionId);
                List<EasSubmissionValue> submissionValuesList = BuildEasSubmissionValues(validRecords, paymentTypes, submissionId);
                await _easSubmissionService.PersistEasSubmissionAsync(submissionList, submissionValuesList, fileInfo.UKPRN, cancellationToken);
            }

            //_validationService.LogValidationErrors(validationErrorModels, fileInfo);
            await _reportingController.ProduceReportsAsync(easCsvRecords, validationErrorModels, fileInfo, cancellationToken);
        }

        private static List<EasSubmission> BuildSubmissionList(EasFileInfo fileInfo, IList<EasCsvRecord> easCsvRecords, Guid submissionId)
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
                var easSubmission = new EasSubmission
                {
                    SubmissionId = submissionId,
                    CollectionPeriod = collectionPeriod,
                    DeclarationChecked = true,
                    NilReturn = false,
                    ProviderName = string.Empty,
                    Ukprn = fileInfo.UKPRN,
                    UpdatedOn = DateTime.Now
                };
                submissionList.Add(easSubmission);
            }

            return submissionList;
        }

        private static List<EasSubmissionValue> BuildEasSubmissionValues(IList<EasCsvRecord> easCsvRecords, List<PaymentType> paymentTypes, Guid submissionId)
        {
            var submissionValuesList = new List<EasSubmissionValue>();
            foreach (var easRecord in easCsvRecords)
            {
                var paymentType = paymentTypes.FirstOrDefault(x => x.FundingLine.Name == easRecord.FundingLine
                                                                   && x.AdjustmentType.Name == easRecord.AdjustmentType);
                if (paymentType is null)
                {
                    throw new ArgumentOutOfRangeException(nameof(easCsvRecords), $"Funding Line : {easRecord.FundingLine} , AdjustmentType combination :  {easRecord.AdjustmentType}  does not exist.");
                }

                var easSubmissionValues = new EasSubmissionValue
                {
                    PaymentId = paymentType.PaymentId,
                    CollectionPeriod =
                        CollectionPeriodHelper.GetCollectionPeriod(Convert.ToInt32(easRecord.CalendarYear), Convert.ToInt32(easRecord.CalendarMonth)),
                    PaymentValue = decimal.Parse(easRecord.Value),
                    SubmissionId = submissionId
                };
                submissionValuesList.Add(easSubmissionValues);
            }

            return submissionValuesList;
        }

        private List<EasCsvRecord> GetValidRows(IList<EasCsvRecord> data, List<ValidationErrorModel> validationErrorModels)
        {
            var easCsvRecords = data.Where(model => !validationErrorModels.Any(e => e.FundingLine == model.FundingLine
                                                                               && e.AdjustmentType == model.AdjustmentType
                                                                               && e.CalendarYear == model.CalendarYear
                                                                               && e.CalendarMonth == model.CalendarMonth)).ToList();
            return easCsvRecords;
        }
    }
}
