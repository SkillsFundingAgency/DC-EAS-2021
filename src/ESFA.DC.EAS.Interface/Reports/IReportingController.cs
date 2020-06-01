using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;

namespace ESFA.DC.EAS.Interface.Reports
{
    public interface IReportingController
    {
        Task ProduceReportsAsync(
            IEasJobContext easContext,
            IEnumerable<EasCsvRecord> models,
            IEnumerable<ValidationErrorModel> errors,
            CancellationToken cancellationToken);

        Task FileLevelErrorReportAsync(
            IEasJobContext easContext,
            IEnumerable<EasCsvRecord> models,
            IEnumerable<ValidationErrorModel> errors,
            CancellationToken cancellationToken);
    }
}
