using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.EAS.DataService.Interface.Postcodes
{
    public interface IPostcodesDataService
    {
        Task<IReadOnlyDictionary<int, string>> GetMcaShortCodesForSofCodesAsync(IEnumerable<int> sofCodes, CancellationToken cancellationToken);
    }
}
