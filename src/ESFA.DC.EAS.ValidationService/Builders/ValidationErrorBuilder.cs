using System;
using System.Collections.Generic;
using ESFA.DC.EAS.Model;
using FluentValidation.Results;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.EAS.ValidationService.Builders.Interface;

namespace ESFA.DC.EAS.ValidationService.Builders
{
    public class ValidationErrorBuilder : IValidationErrorBuilder
    {
        public ICollection<ValidationErrorModel> BuildFileLevelValidationErrors(ICollection<ValidationErrorModel> errorModels, IReadOnlyDictionary<string, ValidationErrorRule> validationErrorReferenceData)
        {
            foreach (var error in errorModels)
            {
                error.Severity = validationErrorReferenceData[error.RuleName].Severity;
                error.ErrorMessage = validationErrorReferenceData[error.RuleName].Message;
            }

            return errorModels;
        }

        public ICollection<ValidationErrorModel> BuildValidationErrors(ICollection<ValidationResult> validationResults, IReadOnlyDictionary<string, ValidationErrorRule> validationErrorReferenceData)
        {
            var validationErrorModelList = new List<ValidationErrorModel>();
            foreach (var result in validationResults)
            {
                foreach (var error in result.Errors)
                {
                    if (error.CustomState.GetType() == typeof(EasCsvRecord))
                    {
                        var record = (EasCsvRecord)error.CustomState;
                        var errorModel = BuildValidationErrorModel(record, error.ErrorCode, validationErrorReferenceData[error.ErrorCode]);
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
                                var errorModel = BuildValidationErrorModel(record, error.ErrorCode, validationErrorReferenceData[error.ErrorCode]);
                                validationErrorModelList.Add(errorModel);
                            }
                        }
                    }
                }
            }

            return validationErrorModelList;
        }

        private ValidationErrorModel BuildValidationErrorModel(EasCsvRecord record, string errorCode, ValidationErrorRule validationErrorRule)
        {
            if (validationErrorRule == null)
            {
                throw new Exception($"ValidationErrorRule Not found for the error code:  {errorCode}");
            }

            var errorModel = new ValidationErrorModel()
            {
                FundingLine = record.FundingLine,
                AdjustmentType = record.AdjustmentType,
                CalendarYear = record.CalendarYear,
                CalendarMonth = record.CalendarMonth,
                Value = record.Value,
                ErrorMessage = validationErrorRule.Message,
                RuleName = errorCode,
                Severity = validationErrorRule.Severity,
                DevolvedAreaSoF = record.DevolvedAreaSourceOfFunding
            };

            return errorModel;
        }
    }
}
