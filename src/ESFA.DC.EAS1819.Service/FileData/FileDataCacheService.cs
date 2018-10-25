using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Interface.FileData;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.EAS1819.Service.FileData
{
    public class FileDataCacheService : IFileDataCacheService
    {
        private const string FILEDATA_BY_UKPRN_KEY = "FILEDATA.UKPRN-{0}";

        private readonly IKeyValuePersistenceService _keyValuePersistenceService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly ILogger _logger;

        public FileDataCacheService(IKeyValuePersistenceService keyValuePersistenceService, IJsonSerializationService jsonSerializationService, ILogger logger)
        {
            _keyValuePersistenceService = keyValuePersistenceService;
            _jsonSerializationService = jsonSerializationService;
            _logger = logger;
        }

        public async Task<IFileDataCache> GetFileDataCacheAsync(string Ukprn, CancellationToken cancellationToken)
        {
            string fileDataString;
            string key = string.Format(FILEDATA_BY_UKPRN_KEY, Ukprn);
            try
            {
                fileDataString = await _keyValuePersistenceService.GetAsync(key, cancellationToken);
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                _logger.LogError("Key '" + key + "' was not found in the store");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("File Data Cache retrieval exception for the Key '" + key + "' ", ex);
                throw ex;
            }

            var fileDataCache = _jsonSerializationService.Deserialize<FileDataCache>(fileDataString);
            _logger.LogInfo("Key '" + key + "' found in the store");
            return fileDataCache;
        }

        public async Task PopulateFileDataCacheAsync(IFileDataCache fileDataCache, CancellationToken cancellationToken)
        {
            string key = string.Format(FILEDATA_BY_UKPRN_KEY, fileDataCache.UkPrn);
            var fileDataSerialized = _jsonSerializationService.Serialize(fileDataCache);
            await _keyValuePersistenceService.SaveAsync(key, fileDataSerialized, cancellationToken);
        }
    }
}
