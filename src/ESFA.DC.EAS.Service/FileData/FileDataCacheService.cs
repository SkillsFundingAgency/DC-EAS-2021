using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.Model;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.EAS.Service.FileData
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

        public async Task<IFileDataCache> GetFileDataCacheAsync(int ukPrn, CancellationToken cancellationToken)
        {
            string fileDataString;
            string key = string.Format(FILEDATA_BY_UKPRN_KEY, ukPrn.ToString());
            try
            {
                fileDataString = await _keyValuePersistenceService.GetAsync(key, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"File Data Cache retrieval exception for the Key \'{key}\' ", ex);
                throw;
            }

            var fileDataCache = _jsonSerializationService.Deserialize<FileDataCache>(fileDataString);
            _logger.LogInfo($"Key \'{key}\' found in the store");
            return fileDataCache;
        }

        public async Task PopulateFileDataCacheAsync(IFileDataCache fileDataCache, CancellationToken cancellationToken)
        {
            string key = string.Format(FILEDATA_BY_UKPRN_KEY, fileDataCache.UkPrn);
            var fileDataSerialized = _jsonSerializationService.Serialize(fileDataCache);
            await _keyValuePersistenceService.SaveAsync(key, fileDataSerialized, cancellationToken);
        }

        public IFileDataCache BuildFileDataCache(
           int ukprn,
           string filename,
           IEnumerable<EasCsvRecord> easCsvRecords,
           IEnumerable<EasCsvRecord> validRecords,
           IEnumerable<ValidationErrorModel> validationErrorModels,
           bool failedFileValidation)
        {
            FileDataCache fileDataCache = new FileDataCache()
            {
                UkPrn = ukprn.ToString(),
                Filename = filename,
                AllEasCsvRecords = easCsvRecords,
                ValidEasCsvRecords = validRecords,
                ValidationErrors = validationErrorModels,
                FailedFileValidation = failedFileValidation
            };

            return fileDataCache;
        }
    }
}
