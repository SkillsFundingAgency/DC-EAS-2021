using ESFA.DC.EAS.ValidationService.Extensions;
using ESFA.DC.EAS1920.EF;

namespace ESFA.DC.EAS.ValidationService.Builders
{
    using System.Collections.Generic;
    using ESFA.DC.EAS.Model;
    using FluentValidation.Results;

    public class ValidationErrorBuilder
    {
        public static List<ValidationErrorModel> BuildValidationErrors(List<ValidationResult> validationResults, List<ValidationErrorRule> validationErrorRules)
        {
            var validationErrorModelList = new List<ValidationErrorModel>();
            foreach (var result in validationResults)
            {
                foreach (var error in result.Errors)
                {
                    if (error.CustomState.GetType() == typeof(EasCsvRecord))
                    {
                        var record = (EasCsvRecord)error.CustomState;
                        var errorModel = record.ToValidationErrorModel(error, validationErrorRules);
                        validationErrorModelList.Add(errorModel);
                    }
                    else if (error.CustomState.GetType() == typeof(Dictionary<List<EasCsvRecord>, int>))
                    {
                        IDictionary<List<EasCsvRecord>, int> stateRecords = (IDictionary<List<EasCsvRecord>, int>)error.CustomState;
                        foreach (var stateRecord in stateRecords)
                        {
                            var easRecords = stateRecord.Key;

                            foreach (var record in easRecords)
                            {
                                var errorModel = record.ToValidationErrorModel(error, validationErrorRules);
                                validationErrorModelList.Add(errorModel);
                            }
                        }
                    }
                }
            }

            return validationErrorModelList;
        }
    }
}
