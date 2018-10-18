namespace ESFA.DC.EAS1819.Service.Import
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac.Features.AttributeFilters;
    using ESFA.DC.EAS1819.DataService.Interface;
    using ESFA.DC.EAS1819.EF;
    using ESFA.DC.EAS1819.Interface;
    using ESFA.DC.EAS1819.Interface.Reports;
    using ESFA.DC.EAS1819.Interface.Validation;
    using ESFA.DC.EAS1819.Model;
    using ESFA.DC.EAS1819.Service.Helpers;
    using ESFA.DC.EAS1819.Service.Interface;
    using ESFA.DC.EAS1819.Service.Mapper;
    using ESFA.DC.IO.Interfaces;
    using ESFA.DC.Logging.Interfaces;

    public class ImportService : IImportService
    {
        private readonly Guid _submissionId;
        private readonly IRepository<PaymentTypes> _paymentTypeRepository;
        private readonly IEasSubmissionService _easSubmissionService;
        private readonly IEasPaymentService _easPaymentService;
        private readonly IEASDataProviderService _easDataProviderService;
        private readonly ICsvParser _csvParser;
        private readonly IValidationService _validationService;
        private readonly IReportingController _reportingController;
        private readonly IStreamableKeyValuePersistenceService _keyValuePersistenceService;
        private readonly ILogger _logger;

        public ImportService(
            IEasSubmissionService easSubmissionService,
            IEasPaymentService easPaymentService,
            IEASDataProviderService easDataProviderService,
            ICsvParser csvParser,
            IValidationService validationService,
            IReportingController reportingController,
            [KeyFilter(PersistenceStorageKeys.AzureStorage)]IStreamableKeyValuePersistenceService keyValuePersistenceService,
            ILogger logger)
        {
            _easSubmissionService = easSubmissionService;
            _easPaymentService = easPaymentService;
            _easDataProviderService = easDataProviderService;
            _csvParser = csvParser;
            _validationService = validationService;
            _reportingController = reportingController;
            _keyValuePersistenceService = keyValuePersistenceService;
            _logger = logger;
        }

        public ImportService(
            Guid submissionId,
            IEasSubmissionService easSubmissionService,
            IEasPaymentService easPaymentService,
            IEASDataProviderService easDataProviderService,
            ICsvParser csvParser,
            IValidationService validationService,
            IReportingController reportingController,
            [KeyFilter(PersistenceStorageKeys.AzureStorage)]IStreamableKeyValuePersistenceService keyValuePersistenceService,
            ILogger logger)
            : this(easSubmissionService, easPaymentService, easDataProviderService, csvParser, validationService, reportingController, keyValuePersistenceService, logger)
        {
            _submissionId = submissionId;
        }

        public async Task ImportEasDataAsync(EasFileInfo fileInfo, IList<EasCsvRecord> easCsvRecords, CancellationToken cancellationToken)
        {
           //StreamReader streamReader;
            //try
            //{
            //    streamReader = await _easDataProviderService.Provide(fileInfo, cancellationToken);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"Azure service provider failed to return stream, key: {fileInfo.FileName}", ex);
            //    throw ex;
            //}

            //cancellationToken.ThrowIfCancellationRequested();

            //using (streamReader)
            //{
            //    var validationErrorModel = _validationService.ValidateFile(streamReader, out easCsvRecords);
            //    if (validationErrorModel.ErrorMessage != null)
            //    {
            //        _validationService.LogValidationErrors(new List<ValidationErrorModel> { validationErrorModel }, fileInfo);
            //        _logger.LogError($"The file format is incorrect.  Please check the field headers are as per the Guidance document. File: {fileInfo.FileName}");
            //        await _reportingController.FileLevelErrorReport(
            //            null,
            //            fileInfo,
            //            new List<ValidationErrorModel> { validationErrorModel },
            //            cancellationToken);
            //        return;
            //    }
            //}

            var validationErrorModels = _validationService.ValidateData(fileInfo, easCsvRecords.ToList());
            cancellationToken.ThrowIfCancellationRequested();
            var validRecords = GetValidRows(easCsvRecords, validationErrorModels);
            if (validRecords.Count > 0)
            {
                var paymentTypes = _easPaymentService.GetAllPaymentTypes();
                var submissionId = _submissionId != (Guid.Empty) ? _submissionId : Guid.NewGuid();
                var submissionList = BuildSubmissionList(fileInfo, validRecords, submissionId);
                var submissionValuesList = BuildEasSubmissionValues(validRecords, paymentTypes, submissionId);
                await _easSubmissionService.PersistEasSubmissionAsync(submissionList, submissionValuesList, cancellationToken);
            }

            _validationService.LogValidationErrors(validationErrorModels, fileInfo);
            await _reportingController.ProduceReports(easCsvRecords, validationErrorModels, fileInfo, cancellationToken);
        }

        private static List<EasSubmission> BuildSubmissionList(EasFileInfo fileInfo, IList<EasCsvRecord> easCsvRecords, Guid submissionId)
        {
            List<int> distinctCollectionPeriods = new List<int>();
            var submissionList = new List<EasSubmission>();
            foreach (var record in easCsvRecords)
            {
                distinctCollectionPeriods.Add(
                    CollectionPeriodHelper.GetCollectionPeriod(record.CalendarYear, record.CalendarMonth));
            }

            distinctCollectionPeriods = distinctCollectionPeriods.Distinct().ToList();

            var list = easCsvRecords.Select(p => new
            {
                collectionPeriod = CollectionPeriodHelper.GetCollectionPeriod(p.CalendarYear, p.CalendarMonth)
            }).Distinct().ToList();

            foreach (var collectionPeriod in distinctCollectionPeriods)
            {
                var easSubmission = new EasSubmission()
                {
                    SubmissionId = submissionId,
                    CollectionPeriod = collectionPeriod,
                    DeclarationChecked = true,
                    NilReturn = false,
                    ProviderName = string.Empty,
                    Ukprn = fileInfo.UKPRN,
                    UpdatedOn = DateTime.Now,
                };
                submissionList.Add(easSubmission);
            }

            return submissionList;
        }

        private static List<EasSubmissionValues> BuildEasSubmissionValues(IList<EasCsvRecord> easCsvRecords, List<PaymentTypes> paymentTypes, Guid submissionId)
        {
            var submissionValuesList = new List<EasSubmissionValues>();
            foreach (var easRecord in easCsvRecords)
            {
                var paymentType = paymentTypes.FirstOrDefault(x => x.FundingLine == easRecord.FundingLine
                                                                   && x.AdjustmentType == easRecord.AdjustmentType);
                if (paymentType is null)
                {
                    throw new Exception(
                        $"Funding Line : {easRecord.FundingLine} , AdjustmentType combination :  {easRecord.AdjustmentType}  does not exist.");
                }

                var easSubmissionValues = new EasSubmissionValues()
                {
                    PaymentId = paymentType.PaymentId,
                    CollectionPeriod =
                        CollectionPeriodHelper.GetCollectionPeriod(easRecord.CalendarYear, easRecord.CalendarMonth),
                    PaymentValue = easRecord.Value,
                    SubmissionId = submissionId,
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
                                                                               && e.CalendarMonth == model.CalendarMonth
                                                                               && e.Value == model.Value)).ToList();
            return easCsvRecords;
        }
    }
}
