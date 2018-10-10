using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Interface.Reports;
using ESFA.DC.EAS1819.Service.Helpers;
using ESFA.DC.IO.Interfaces;

namespace ESFA.DC.EAS1819.Service.Import
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CsvHelper;
    using ESFA.DC.EAS1819.DataService.Interface;
    using ESFA.DC.EAS1819.EF;
    using ESFA.DC.EAS1819.Model;
    using ESFA.DC.EAS1819.Service.Interface;
    using ESFA.DC.EAS1819.Service.Mapper;

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

        public ImportService(
            IEasSubmissionService easSubmissionService,
            IEasPaymentService easPaymentService,
            IEASDataProviderService easDataProviderService,
            ICsvParser csvParser,
            IValidationService validationService,
            IReportingController reportingController,
            [KeyFilter(PersistenceStorageKeys.AzureStorage)]IStreamableKeyValuePersistenceService keyValuePersistenceService)
        {
            _easSubmissionService = easSubmissionService;
            _easPaymentService = easPaymentService;
            _easDataProviderService = easDataProviderService;
            _csvParser = csvParser;
            _validationService = validationService;
            _reportingController = reportingController;
            _keyValuePersistenceService = keyValuePersistenceService;
        }

        public ImportService(
            Guid submissionId,
            IEasSubmissionService easSubmissionService,
            IEasPaymentService easPaymentService,
            IEASDataProviderService easDataProviderService,
            ICsvParser csvParser,
            IValidationService validationService,
            IReportingController reportingController,
            [KeyFilter(PersistenceStorageKeys.AzureStorage)]IStreamableKeyValuePersistenceService keyValuePersistenceService)
            : this(easSubmissionService, easPaymentService, easDataProviderService, csvParser, validationService, reportingController, keyValuePersistenceService)
        {
            _submissionId = submissionId;
        }

        public async Task ImportEasData(EasFileInfo fileInfo, CancellationToken cancellationToken)
        {
            IList<EasCsvRecord> easCsvRecords;
            var paymentTypes = _easPaymentService.GetAllPaymentTypes();
            var streamReader = _easDataProviderService.Provide(fileInfo, CancellationToken.None).Result;

            using (streamReader)
            {
                var headers = _csvParser.GetHeaders(streamReader);
                var validationErrorModel = _validationService.ValidateHeader(headers);
                if (validationErrorModel.ErrorMessage != null)
                {
                    throw new InvalidDataException("Invalid Headers");
                }

                streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                easCsvRecords = _csvParser.GetData(streamReader, new EasCsvRecordMapper());
            }

            var validationErrorModels = _validationService.ValidateData(easCsvRecords.ToList());

            if (validationErrorModels.Count <= 0)
            {
                var submissionId = _submissionId != (Guid.Empty) ? _submissionId : Guid.NewGuid();
                //var easSubmission = new EasSubmission()
                //{
                //    SubmissionId = submissionId,
                //    CollectionPeriod = 7,
                //    DeclarationChecked = true,
                //    NilReturn = false,
                //    ProviderName = "MK",
                //    Ukprn = "123465",
                //    UpdatedOn = DateTime.Now,
                //};
                var submissionValuesList = new List<EasSubmissionValues>();
                foreach (var easRecord in easCsvRecords)
                {
                    var paymentType = paymentTypes.FirstOrDefault(x => x.FundingLine == easRecord.FundingLine
                                                                       && x.AdjustmentType == easRecord.AdjustmentType);
                    if (paymentType is null)
                    {
                        throw new Exception($"Funding Line : {easRecord.FundingLine} , AdjustmentType combination :  {easRecord.AdjustmentType}  does not exist.");
                    }

                    var easSubmissionValues = new EasSubmissionValues()
                    {
                        PaymentId = paymentType.PaymentId,
                        CollectionPeriod = CollectionPeriodHelper.GetCollectionPeriod(easRecord.CalendarYear, easRecord.CalendarMonth),
                        PaymentValue = easRecord.Value,
                        SubmissionId = submissionId,
                    };
                    submissionValuesList.Add(easSubmissionValues);
                }

                //easSubmission.SubmissionValues = submissionValuesList;
                //_easSubmissionService.PersistEasSubmission(easSubmission);
                _easSubmissionService.PersistEasSubmissionValues(submissionValuesList);
            }
            else
            {
                _validationService.LogValidationErrors(validationErrorModels, fileInfo);
                await _reportingController.ProduceReports(easCsvRecords, validationErrorModels, fileInfo, cancellationToken);
            }
        }
    }
}
