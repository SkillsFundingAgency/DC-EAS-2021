using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;

namespace ESFA.DC.EAS.Interface.Validation
{
   public interface IValidationService
   {
       Task<List<ValidationErrorModel>> ValidateDataAsync(IEasJobContext easContext, CancellationToken cancellationToken);
   }
}
