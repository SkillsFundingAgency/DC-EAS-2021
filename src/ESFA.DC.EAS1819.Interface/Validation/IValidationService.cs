using System.Collections.Generic;
using System.IO;
using ESFA.DC.EAS1819.Model;

namespace ESFA.DC.EAS1819.Interface.Validation
{
   public interface IValidationService
   {
        //ValidationErrorModel ValidateHeader(IList<string> headers);
       ValidationErrorModel ValidateFile(StreamReader streamReader, out IList<EasCsvRecord> easCsvRecords);

       List<ValidationErrorModel> ValidateData(EasFileInfo fileInfo, List<EasCsvRecord> easCsvRecords);

       void LogValidationErrors(List<ValidationErrorModel> validationErrors, EasFileInfo fileInfo);
   }
}
