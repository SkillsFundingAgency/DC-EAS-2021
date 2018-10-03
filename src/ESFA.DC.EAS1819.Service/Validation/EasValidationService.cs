﻿using System;
using ESFA.DC.EAS1819.Service.Helpers;

namespace ESFA.DC.EAS1819.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using ESFA.DC.DateTimeProvider.Interface;
    using ESFA.DC.EAS1819.DataService.Interface;
    using ESFA.DC.EAS1819.EF;
    using ESFA.DC.EAS1819.Model;
    using ESFA.DC.EAS1819.Service.Extensions;
    using ESFA.DC.EAS1819.Service.Interface;
    using ESFA.DC.EAS1819.Service.Validation;
    using FluentValidation;
    using FluentValidation.Results;

    public class EasValidationService : IValidationService
    {
        private readonly IEasPaymentService _easPaymentService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IValidationErrorService _validationErrorService;
        private readonly IValidatorFactory _validatorFactory;
        FileValidator _fileValidator;

        public EasValidationService(
            IEasPaymentService easPaymentService,
            IDateTimeProvider dateTimeProvider,
            IValidationErrorService validationErrorService)
        {
            _easPaymentService = easPaymentService;
            _dateTimeProvider = dateTimeProvider;
            _validationErrorService = validationErrorService;
            _fileValidator = new FileValidator();
        }

        public ValidationErrorModel ValidateHeader(IList<string> headers)
        {
            var validationErrorModel = new ValidationErrorModel();
            var validationResult = _fileValidator.Validate(headers);
            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.FirstOrDefault();
                validationErrorModel = new ValidationErrorModel()
                {
                    ErrorMessage = error.ErrorMessage,
                    RuleName = error.ErrorCode
                };
            }

            return validationErrorModel;
        }

        public List<ValidationErrorModel> ValidateData(List<EasCsvRecord> easCsvRecords)
        {
            var validationResults = new List<ValidationResult>();
            var businessRulesValidationResults = new List<ValidationResult>();
            List<PaymentTypes> paymentTypes = _easPaymentService.GetAllPaymentTypes();

            // Business Rule validators
            foreach (var easRecord in easCsvRecords)
            {
                var validate = new BusinessRulesValidator(_dateTimeProvider, paymentTypes).Validate(easRecord);
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
                    Severity = error.IsWarning ? "W" : "E",
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
