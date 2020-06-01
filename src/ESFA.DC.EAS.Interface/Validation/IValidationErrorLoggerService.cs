using ESFA.DC.EAS.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.EAS.Interface.Validation
{
    public interface IValidationErrorLoggerService
    {
        Task LogValidationErrorsAsync(IEasJobContext easJobContext, IEnumerable<ValidationErrorModel> validationErrors, CancellationToken cancellationToken);
    }
}
