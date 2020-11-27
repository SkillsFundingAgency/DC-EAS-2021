using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS2021.EF;

namespace ESFA.DC.EAS.Interface.Validation
{
   public interface IFileValidationService
   {
       Task<List<ValidationErrorModel>> ValidateFile(IEasJobContext easContext, IReadOnlyDictionary<string, ValidationErrorRule> validationErrorReferenceData, CancellationToken cancellationToken);
   }
}
