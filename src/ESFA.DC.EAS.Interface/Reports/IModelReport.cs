using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;

namespace ESFA.DC.EAS.Interface.Reports
{
    public interface IModelReport
    {
        Task<IEnumerable<string>> GenerateReportAsync(
            IEasJobContext easContext,
            IEnumerable<EasCsvRecord> data,
            IEnumerable<ValidationErrorModel> validationErrors,
            CancellationToken cancellationToken);
    }
}
