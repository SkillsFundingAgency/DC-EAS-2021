using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;

namespace ESFA.DC.EAS.Interface
{
    public interface IEASFileDataProviderService
    {
        Task<List<EasCsvRecord>> ProvideData(string fileName, string container, CancellationToken cancellationToken);
    }
}
