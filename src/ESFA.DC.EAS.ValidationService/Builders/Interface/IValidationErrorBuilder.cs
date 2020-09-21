using System.Collections.Generic;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS2021.EF;
using FluentValidation.Results;

namespace ESFA.DC.EAS.ValidationService.Builders.Interface
{
    public interface IValidationErrorBuilder
    {
        ICollection<ValidationErrorModel> BuildValidationErrors(ICollection<ValidationResult> validationResults, IReadOnlyDictionary<string, ValidationErrorRule> validationErrorReferenceData);
        ICollection<ValidationErrorModel> BuildFileLevelValidationErrors(ICollection<ValidationErrorModel> errorModels, IReadOnlyDictionary<string, ValidationErrorRule> validationErrorReferenceData);
    }
}
