using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS2021.EF;

namespace ESFA.DC.EAS.Interface.Validation
{
   public interface IValidationService
   {
       Task<List<ValidationErrorModel>> ValidateDataAsync(IEasJobContext easContext, IReadOnlyDictionary<string, ValidationErrorRule> validationErrorReferenceData, CancellationToken cancellationToken);
   }
}
