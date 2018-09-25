namespace ESFA.DC.EAS1819.Service
{
    using System.Collections.Generic;
    using ESFA.DC.EAS1819.DataService.Interface;
    using ESFA.DC.EAS1819.Model;
    using ESFA.DC.EAS1819.Service.Interface;
    using ESFA.DC.EAS1819.Service.Validation;
    using FluentValidation;
    using FluentValidation.Results;

    public class EasValidationService : IValidationService
    {
        private readonly IEasPaymentService _easPaymentService;
        private readonly IValidatorFactory _validatorFactory;
        FileValidator _fileValidator;

        public EasValidationService(IEasPaymentService easPaymentService)
        {
            _easPaymentService = easPaymentService;
            _fileValidator = new FileValidator();
        }

        public ValidationResult ValidateHeader(IList<string> headers)
        {
            var validationResult = _fileValidator.Validate(headers);
            return validationResult;
        }

        public List<ValidationResult> ValidateData(List<EasCsvRecord> easCsvRecords)
        {
            var validationResults = new List<ValidationResult>();
            var businessRulesValidationResults = new List<ValidationResult>();

            // Business Rule validators
            foreach (var easRecord in easCsvRecords)
            {
                var validate = new BusinessRulesValidator().Validate(easRecord);
                businessRulesValidationResults.Add(validate);
            }

           validationResults.AddRange(businessRulesValidationResults);
            return validationResults;
        }
    }
}
