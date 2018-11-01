using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.EF;

namespace ESFA.DC.EAS1819.DataService.Interface
{
    public interface IValidationErrorService
    {
        Task<int> LogErrorSourceFileAsync(SourceFile sourceFile);

       Task<List<ValidationError>> GetValidationErrorsAsync(string UkPrn);

        Task LogValidationErrorsAsync(List<ValidationError> validationErrors, CancellationToken cancellationToken);
    }
}
