using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.EAS1819.Interface
{
    public interface IEASDataProviderService
    {
        Task<IList<EasCsvRecord>> ProvideData(IJobContextMessage jobContextMessage, CancellationToken cancellationToken);

        Task<StreamReader> ProvideAsync(EasFileInfo easFileInfo, CancellationToken cancellationToken);
    }
}
