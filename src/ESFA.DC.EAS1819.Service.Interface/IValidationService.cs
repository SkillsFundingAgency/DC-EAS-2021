using System.Collections.Generic;
using ESFA.DC.EAS1819.Model;
using FluentValidation.Results;

namespace ESFA.DC.EAS1819.Service.Interface
{
   public interface IValidationService
   {
       ValidationErrorModel ValidateHeader(IList<string> headers);

       List<ValidationErrorModel> ValidateData(List<EasCsvRecord> easCsvRecords);

       void LogValidationErrors(List<ValidationErrorModel> validationErrors, EasFileInfo fileInfo);
       //List<ValidationResult> Validate<T>(T entity)
       //     where T : EasCsvData;
   }
}
