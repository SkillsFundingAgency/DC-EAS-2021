using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.EAS.Interface.FileData
{
    public interface IFileDataCacheService
    {
        Task<IFileDataCache> GetFileDataCacheAsync(string ukPrn, CancellationToken cancellationToken);

        Task PopulateFileDataCacheAsync(IFileDataCache fileDataCache, CancellationToken cancellationToken);
    }
}
