using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.EF;

namespace ESFA.DC.EAS1819.DataService.Interface
{
    public interface IValidationErrorService
    {
        Task<int> LogErrorSourceFileAsync(SourceFile sourceFile, CancellationToken cancellationToken);

        Task<List<ValidationError>> GetValidationErrorsAsync(string ukPrn, CancellationToken cancellationToken);

        Task LogValidationErrorsAsync(List<ValidationError> validationErrors, CancellationToken cancellationToken);
    }
}
