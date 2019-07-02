using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.EAS1819.Interface.FileData
{
    public interface IFileDataCacheService
    {
        Task<IFileDataCache> GetFileDataCacheAsync(string ukPrn, CancellationToken cancellationToken);

        Task PopulateFileDataCacheAsync(IFileDataCache fileDataCache, CancellationToken cancellationToken);
    }
}
