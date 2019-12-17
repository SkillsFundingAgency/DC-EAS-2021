using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.EAS.Interface.Reports
{
    public interface IModelReport
    {
        Task<IEnumerable<string>> GenerateReportAsync(
            IJobContextMessage jobContextMessage,
            IList<EasCsvRecord> data,
            EasFileInfo fileInfo,
            IList<ValidationErrorModel> validationErrors,
            CancellationToken cancellationToken);
    }
}
