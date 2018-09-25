using System.Collections.Generic;
using ESFA.DC.EAS1819.Model;
using FluentValidation.Results;

namespace ESFA.DC.EAS1819.Service.Interface
{
   public interface IValidationService
   {
       ValidationResult ValidateHeader(IList<string> headers);

       List<ValidationResult> ValidateData(List<EasCsvRecord> easCsvRecords);
       //List<ValidationResult> Validate<T>(T entity)
       //     where T : EasCsvData;
   }
}
