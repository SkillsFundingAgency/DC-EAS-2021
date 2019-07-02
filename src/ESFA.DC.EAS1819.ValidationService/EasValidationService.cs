using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.DataService.Interface.FCS;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Interface.Validation;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.ValidationService.Builders;
using ESFA.DC.EAS1819.ValidationService.Mapper;
using ESFA.DC.EAS1819.ValidationService.Validators;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.FCS.Model;
using FluentValidation;
using FluentValidation.Results;

namespace ESFA.DC.EAS1819.ValidationService
{
    public class EasValidationService : IValidationService
    {
        private readonly IEasPaymentService _easPaymentService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICsvParser _csvParser;
        private readonly IValidationErrorService _validationErrorService;
        private readonly IFCSDataService _fcsDataService;
        private readonly IFundingLineContractTypeMappingDataService _fundingLineContractTypeMappingDataService;
        private readonly IValidationErrorRuleService _validationErrorRuleService;
        private readonly ILogger _logger;
        private readonly IValidatorFactory _validatorFactory;

        public EasValidationService(
            IEasPaymentService easPaymentService,
            IDateTimeProvider dateTimeProvider,
            IValidationErrorService validationErrorService,
            IFCSDataService fcsDataService,
            IFundingLineContractTypeMappingDataService fundingLineContractTypeMappingDataService,
            IValidationErrorRuleService validationErrorRuleService,
            ILogger logger)
        {
            _easPaymentService = easPaymentService;
            _dateTimeProvider = dateTimeProvider;
            _validationErrorService = validationErrorService;
            _fcsDataService = fcsDataService;
            _fundingLineContractTypeMappingDataService = fundingLineContractTypeMappingDataService;
            _validationErrorRuleService = validationErrorRuleService;
            _logger = logger;
        }

        public ValidationErrorModel ValidateFile(StreamReader streamReader, out List<EasCsvRecord> easCsvRecords)
        {
            ValidationErrorModel validationErrorModel = new ValidationErrorModel();
            try
            {
                streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                var csv = new CsvReader(streamReader);
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.IgnoreBlankLines = true;
                csv.Configuration.TrimOptions = TrimOptions.Trim;
                csv.Configuration.RegisterClassMap(new EasCsvRecordMapper());
                csv.Read();
                csv.ReadHeader();
                csv.ValidateHeader(typeof(EasCsvRecord));
                easCsvRecords = csv.GetRecords<EasCsvRecord>().ToList();
            }
            catch (Exception ex)
            {
                easCsvRecords = null;
                return new ValidationErrorModel()
                {
                    Severity = "E",
                    RuleName = "Fileformat_01",
                    ErrorMessage = "The file format is incorrect.  Please check the field headers are as per the Guidance document."
                };
            }

            return validationErrorModel;
        }

        public async Task<List<ValidationErrorModel>> ValidateDataAsync(EasFileInfo fileInfo, List<EasCsvRecord> easCsvRecords, CancellationToken cancellationToken)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            List<ValidationResult> businessRulesValidationResults = new List<ValidationResult>();
            cancellationToken.ThrowIfCancellationRequested();
            List<PaymentType> paymentTypes = await _easPaymentService.GetAllPaymentTypes(cancellationToken);
            List<ValidationErrorRule> validationErrorRules = await _validationErrorRuleService.GetAllValidationErrorRules(cancellationToken);
            List<ContractAllocation> contractsForProvider = _fcsDataService.GetContractsForProvider(int.Parse(fileInfo.UKPRN));
            List<ContractAllocation> validContractAllocations = contractsForProvider.Where(x => fileInfo.DateTime >= x.StartDate && fileInfo.DateTime <= x.EndDate).ToList();
            List<FundingLineContractTypeMapping> fundingLineContractTypeMappings = await _fundingLineContractTypeMappingDataService.GetAllFundingLineContractTypeMappings(cancellationToken);

            BusinessRulesValidator validator = new BusinessRulesValidator(validContractAllocations, fundingLineContractTypeMappings, paymentTypes, _dateTimeProvider, fileInfo.ReturnPeriod);

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

        public async Task LogValidationErrorsAsync(List<ValidationErrorModel> validationErrors, EasFileInfo fileInfo, CancellationToken cancellationToken)
        {
            var validationErrorList = new List<ValidationError>();
            var sourceFile = new SourceFile()
            {
                Ukprn = fileInfo.UKPRN,
                DateTime = fileInfo.DateTime,
                FileName = fileInfo.FileName,
                FilePreparationDate = fileInfo.FilePreparationDate
            };

            int sourceFileId = await _validationErrorService.LogErrorSourceFileAsync(sourceFile, cancellationToken);

            foreach (var error in validationErrors)
            {
                var validationError = new ValidationError()
                {
                    AdjustmentType = error.AdjustmentType,
                    Value = error.Value,
                    CalendarMonth = error.CalendarMonth,
                    CalendarYear = error.CalendarYear,
                    CreatedOn = DateTime.UtcNow,
                    ErrorMessage = error.ErrorMessage,
                    FundingLine = error.FundingLine,
                    RowId = Guid.NewGuid(), //TODO: find out if this is right.
                    RuleId = error.RuleName,
                    Severity = error.Severity,
                    SourceFileId = sourceFileId
                };

                validationErrorList.Add(validationError);
            }

            await _validationErrorService.LogValidationErrorsAsync(validationErrorList, cancellationToken);
        }
    }
}
