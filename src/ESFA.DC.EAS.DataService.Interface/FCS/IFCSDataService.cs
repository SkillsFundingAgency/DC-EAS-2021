using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ReferenceData.FCS.Model;

namespace ESFA.DC.EAS.DataService.Interface.FCS
{
    public interface IFCSDataService
    {
        Task<List<ContractAllocation>> GetContractsForProvider(int ukprn, CancellationToken cancellationToken);

        Task<IReadOnlyDictionary<string, IEnumerable<DevolvedContract>>> GetDevolvedContractsForProvider(int ukprn, CancellationToken cancellationToken);
    }
}
