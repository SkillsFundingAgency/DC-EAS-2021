using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.EAS.Interface
{
    public interface IEASDataProviderService
    {
        Task<IList<EasCsvRecord>> ProvideData(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);

        Task<StreamReader> ProvideAsync(EasFileInfo easFileInfo, CancellationToken cancellationToken);
    }
}
