using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS2021.EF;

namespace ESFA.DC.EAS.DataService.Interface
{
    public interface IValidationErrorService
    {
        Task<int> LogErrorSourceFileAsync(SourceFile sourceFile, CancellationToken cancellationToken);

        Task<List<ValidationError>> GetValidationErrorsAsync(string ukPrn, CancellationToken cancellationToken);

        Task LogValidationErrorsAsync(IEnumerable<ValidationError> validationErrors, CancellationToken cancellationToken);
    }
}
