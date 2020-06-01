using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;

namespace ESFA.DC.EAS.Interface.Validation
{
   public interface IFileValidationService
   {
       Task<List<ValidationErrorModel>> ValidateFile(IEasJobContext easContext, CancellationToken cancellationToken);
   }
}
