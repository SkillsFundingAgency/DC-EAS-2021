using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;

namespace ESFA.DC.EAS.Interface.FileData
{
    public interface IFileDataCacheService
    {
        Task<IFileDataCache> GetFileDataCacheAsync(int ukPrn, CancellationToken cancellationToken);

        Task PopulateFileDataCacheAsync(IFileDataCache fileDataCache, CancellationToken cancellationToken);

        IFileDataCache BuildFileDataCache(
            int ukprn,
            string filename,
            IEnumerable<EasCsvRecord> easCsvRecords,
            IEnumerable<EasCsvRecord> validRecords,
            IEnumerable<ValidationErrorModel> validationErrorModels,
            bool failedFileValidation);
    }
}
