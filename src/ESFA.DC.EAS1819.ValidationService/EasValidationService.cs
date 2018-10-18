using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using CsvHelper;
using ESFA.DC.EAS1819.DataService.Interface.FCS;
using ESFA.DC.EAS1819.Interface.Validation;
using ESFA.DC.EAS1819.Service.Interface;
using ESFA.DC.EAS1819.ValidationService.Builders;
using ESFA.DC.EAS1819.ValidationService.Mapper;
using ESFA.DC.EAS1819.ValidationService.Validators;

namespace ESFA.DC.EAS1819.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using ESFA.DC.DateTimeProvider.Interface;
    using ESFA.DC.EAS1819.DataService.Interface;
    using ESFA.DC.EAS1819.EF;
    using ESFA.DC.EAS1819.Model;

    using FluentValidation;
    using FluentValidation.Results;

    public class EasValidationService : IValidationService
    {
        private readonly IEasPaymentService _easPaymentService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICsvParser _csvParser;
        private readonly IValidationErrorService _validationErrorService;
        private readonly IFCSDataService _fcsDataService;
        private readonly IFundingLineContractTypeMappingDataService _fundingLineContractTypeMappingDataService;
        private readonly IValidatorFactory _validatorFactory;

        public EasValidationService(
            IEasPaymentService easPaymentService,
            IDateTimeProvider dateTimeProvider,
            IValidationErrorService validationErrorService,
            IFCSDataService fcsDataService,
            IFundingLineContractTypeMappingDataService fundingLineContractTypeMappingDataService)
        {
            _easPaymentService = easPaymentService;
            _dateTimeProvider = dateTimeProvider;
            _validationErrorService = validationErrorService;
            _fcsDataService = fcsDataService;
            _fundingLineContractTypeMappingDataService = fundingLineContractTypeMappingDataService;
        }

        public ValidationErrorModel ValidateFile(StreamReader streamReader, out IList<EasCsvRecord> easCsvRecords)
        {
            var validationErrorModel = new ValidationErrorModel();
            using (streamReader)
            {
                try
                {
                    streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                    var csv = new CsvReader(streamReader);
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.IgnoreBlankLines = true;
                    csv.Configuration.RegisterClassMap(new EasCsvRecordMapper());
                    csv.Read();
                    csv.ReadHeader();
                    csv.ValidateHeader(typeof(EasCsvRecord));
                    easCsvRecords = csv.GetRecords<EasCsvRecord>().ToList();
                }
                catch (Exception)
                {
                    easCsvRecords = null;
                    return new ValidationErrorModel()
                    {
                        RuleName = "Fileformat_01",
                        ErrorMessage = "The file format is incorrect.  Please check the field headers are as per the Guidance document."
                    };
                }
            }

            return validationErrorModel;
        }

        public List<ValidationErrorModel> ValidateData(EasFileInfo fileInfo, List<EasCsvRecord> easCsvRecords)
        {
            var validationResults = new List<ValidationResult>();
            var businessRulesValidationResults = new List<ValidationResult>();
            List<PaymentTypes> paymentTypes = _easPaymentService.GetAllPaymentTypes();
            var contractsForProvider = _fcsDataService.GetContractsForProvider(int.Parse(fileInfo.UKPRN));
            var validContractAllocations = contractsForProvider.Where(x => fileInfo.DateTime >= x.StartDate && fileInfo.DateTime <= x.EndDate).ToList();
            var fundingLineContractTypeMappings = _fundingLineContractTypeMappingDataService.GetAllFundingLineContractTypeMappings();
            //_fundingLineContractTypeMappingDataService.GetContractTypesRequired()
            // Business Rule validators
            foreach (var easRecord in easCsvRecords)
            {
                var validate = new BusinessRulesValidator(validContractAllocations, fundingLineContractTypeMappings, paymentTypes, _dateTimeProvider).Validate(easRecord);
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

            var validationErrorList = ValidationErrorBuilder.BuildValidationErrors(validationResults);
            return validationErrorList;
        }

        public void LogValidationErrors(List<ValidationErrorModel> validationErrors, EasFileInfo fileInfo)
        {
            var validationErrorList = new List<ValidationError>();
            var sourceFile = new SourceFile()
            {
                UKPRN = fileInfo.UKPRN,
                DateTime = fileInfo.DateTime,
                FileName = fileInfo.FileName,
                FilePreparationDate = fileInfo.FilePreparationDate
            };

          var sourceFileId = _validationErrorService.LogErrorSourceFile(sourceFile);

            foreach (var error in validationErrors)
            {
                var validationError = new ValidationError()
                {
                    AdjustmentType = error.AdjustmentType,
                    Value = error.Value.ToString(),
                    CalendarMonth = error.CalendarMonth.ToString(),
                    CalendarYear = error.CalendarYear.ToString(),
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

            foreach (var error in validationErrorList)
            {
                _validationErrorService.LogValidationError(error);
            }
        }
    }
}
