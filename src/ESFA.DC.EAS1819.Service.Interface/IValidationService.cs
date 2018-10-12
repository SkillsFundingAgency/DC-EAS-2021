using System.Collections.Generic;
using System.IO.Compression;
using ESFA.DC.EAS1819.Model;
using FluentValidation.Results;

namespace ESFA.DC.EAS1819.Service.Interface
{
   public interface IValidationService
   {
       ValidationErrorModel ValidateHeader(IList<string> headers);

       List<ValidationErrorModel> ValidateData(EasFileInfo fileInfo, List<EasCsvRecord> easCsvRecords);

       void LogValidationErrors(List<ValidationErrorModel> validationErrors, EasFileInfo fileInfo);
   }
}
