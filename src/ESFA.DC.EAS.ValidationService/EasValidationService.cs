using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.DataService.Interface.FCS;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Validation;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ValidationService.Builders;
using ESFA.DC.EAS.ValidationService.Validators;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.FCS.Model;
using FluentValidation.Results;
using ESFA.DC.EAS.Interface.Constants;
using ESFA.DC.FileService.Interface;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.DataService.Interface.Postcodes;
using ESFA.DC.EAS.DataService.Constants;

namespace ESFA.DC.EAS.ValidationService
{
    public class EasValidationService : IValidationService
    {
        private readonly IEasPaymentService _easPaymentService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IValidationErrorService _validationErrorService;
        private readonly IFCSDataService _fcsDataService;
        private readonly IPostcodesDataService _postcodesDataService;
        private readonly IFundingLineContractTypeMappingDataService _fundingLineContractTypeMappingDataService;
        private readonly IValidationErrorRuleService _validationErrorRuleService;
        private readonly IFileService _fileService;
        private readonly IEASFileDataProviderService _easFileDataProviderService;
        private readonly IFileDataCacheService _fileDataCacheService;
        private readonly ILogger _logger;

        public EasValidationService(
            IEasPaymentService easPaymentService,
            IDateTimeProvider dateTimeProvider,
            IValidationErrorService validationErrorService,
            IFCSDataService fcsDataService,
            IPostcodesDataService postcodesDataService,
            IFundingLineContractTypeMappingDataService fundingLineContractTypeMappingDataService,
            IValidationErrorRuleService validationErrorRuleService,
            IFileService fileService,
            IEASFileDataProviderService easFileDataProviderService,
            IFileDataCacheService fileDataCacheService,
            ILogger logger)
        {
            _easPaymentService = easPaymentService;
            _dateTimeProvider = dateTimeProvider;
            _validationErrorService = validationErrorService;
            _fcsDataService = fcsDataService;
            _postcodesDataService = postcodesDataService;
            _fundingLineContractTypeMappingDataService = fundingLineContractTypeMappingDataService;
            _validationErrorRuleService = validationErrorRuleService;
            _fileService = fileService;
            _easFileDataProviderService = easFileDataProviderService;
            _fileDataCacheService = fileDataCacheService;
            _logger = logger;
        }

        public async Task<List<ValidationErrorModel>> ValidateDataAsync(IEasJobContext easJobContext, CancellationToken cancellationToken)
        {
            var easCsvRecords = await GetCsvRecords(easJobContext, cancellationToken);
            List<ValidationErrorModel> validationErrorModels = await ValidateAsync(easJobContext, easCsvRecords.ToList(), cancellationToken);

            List<EasCsvRecord> validRecords = GetValidRows(easCsvRecords, validationErrorModels);
            var fileDataCache = _fileDataCacheService.BuildFileDataCache(easJobContext.Ukprn, easJobContext.FileReference, easCsvRecords, validRecords, validationErrorModels, false);
            await _fileDataCacheService.PopulateFileDataCacheAsync(fileDataCache, cancellationToken);

            return validationErrorModels;
        }

        private async Task<List<ValidationErrorModel>> ValidateAsync(IEasJobContext easJobContext, IEnumerable<EasCsvRecord> easCsvRecords, CancellationToken cancellationToken)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            List<ValidationResult> businessRulesValidationResults = new List<ValidationResult>();
            cancellationToken.ThrowIfCancellationRequested();

            List<PaymentType> paymentTypes = await _easPaymentService.GetAllPaymentTypes(cancellationToken);
            List<ValidationErrorRule> validationErrorRules = await _validationErrorRuleService.GetAllValidationErrorRules(cancellationToken);
            List<ContractAllocation> contractsForProvider = await _fcsDataService.GetContractsForProviderAsync(easJobContext.Ukprn, cancellationToken);
            List<FundingLineContractTypeMapping> fundingLineContractTypeMappings = await _fundingLineContractTypeMappingDataService.GetAllFundingLineContractTypeMappings(cancellationToken);

            var devolvedContracts = await _fcsDataService.GetDevolvedContractsForProviderAsync(easJobContext.Ukprn, cancellationToken);
            var sofCodeMcaShortCodeDictionary = await _postcodesDataService.GetMcaShortCodesForSofCodesAsync(DataServiceConstants.ValidDevolvedSourceOfFundingCodes, cancellationToken);

            BusinessRulesValidator validator = new BusinessRulesValidator(contractsForProvider, fundingLineContractTypeMappings, paymentTypes, devolvedContracts, sofCodeMcaShortCodeDictionary, _dateTimeProvider, easJobContext.ReturnPeriod);

            // Business Rule validators
            foreach (EasCsvRecord easRecord in easCsvRecords)
            {
                ValidationResult validate = validator.Validate(easRecord);
                if (!validate.IsValid)
                {
                    businessRulesValidationResults.Add(validate);
                }
            }

            // Cross Record Validation
            var crossRecordValidationResult = new CrossRecordValidator().Validate(easCsvRecords);

            validationResults.AddRange(businessRulesValidationResults);
            if (!crossRecordValidationResult.IsValid)
            {
                validationResults.Add(crossRecordValidationResult);
            }

            var validationErrorList = ValidationErrorBuilder.BuildValidationErrors(validationResults, validationErrorRules);
            return validationErrorList;
        }

        private async Task<List<EasCsvRecord>> GetCsvRecords(IEasJobContext easJobContext, CancellationToken cancellationToken)
        {
            var records = await _easFileDataProviderService.ProvideData(easJobContext.FileReference, easJobContext.Container, cancellationToken);

            return records;
        }

        private List<EasCsvRecord> GetValidRows(IList<EasCsvRecord> data, List<ValidationErrorModel> validationErrorModels)
        {
            var easCsvRecords = data.Where(model => !validationErrorModels.Any(e => e.FundingLine == model.FundingLine
                                                                                    && e.AdjustmentType == model.AdjustmentType
                                                                                    && e.CalendarYear == model.CalendarYear
                                                                                    && e.CalendarMonth == model.CalendarMonth
                                                                                    && e.DevolvedAreaSoF == model.DevolvedAreaSourceOfFunding
                                                                                    && e.Severity == SeverityConstants.Error)).ToList();
            return easCsvRecords;
        }
    }
}
