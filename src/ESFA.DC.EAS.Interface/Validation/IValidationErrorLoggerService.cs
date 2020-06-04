using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.EAS.DataService.Interface
{
    public interface IValidationErrorLoggerService
    {
        Task LogValidationErrorsAsync(IEasJobContext easJobContext, IEnumerable<ValidationErrorModel> validationErrors, CancellationToken cancellationToken);
    }
}
