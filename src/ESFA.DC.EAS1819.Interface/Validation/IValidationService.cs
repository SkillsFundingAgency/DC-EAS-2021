using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Model;

namespace ESFA.DC.EAS1819.Interface.Validation
{
   public interface IValidationService
   {
       ValidationErrorModel ValidateFile(StreamReader streamReader, out IList<EasCsvRecord> easCsvRecords);

       Task<List<ValidationErrorModel>> ValidateDataAsync(EasFileInfo fileInfo, List<EasCsvRecord> easCsvRecords, CancellationToken cancellationToken);

       void LogValidationErrors(List<ValidationErrorModel> validationErrors, EasFileInfo fileInfo);
   }
}
