namespace ESFA.DC.EAS1819.Service.Providers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac.Features.AttributeFilters;
    using CsvHelper;
    using ESFA.DC.EAS1819.Interface;
    using ESFA.DC.EAS1819.Model;
    using ESFA.DC.EAS1819.Service.Interface;
    using ESFA.DC.EAS1819.Service.Mapper;
    using ESFA.DC.IO.Interfaces;
    using ESFA.DC.JobContext.Interface;
    using ESFA.DC.JobContextManager.Model.Interface;
    using ESFA.DC.Logging.Interfaces;

    public class EasAzureStorageDataProviderService : IEASDataProviderService
    {
        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _keyValuePersistenceService;
        private readonly IJobContextMessage _jobContextMessage;
        private readonly CancellationToken _cancellationToken;
        private readonly SemaphoreSlim _getEASLock;

        public EasAzureStorageDataProviderService(
                                                ILogger logger,
                                                [KeyFilter(PersistenceStorageKeys.AzureStorage)]IStreamableKeyValuePersistenceService keyValuePersistenceService)
        {
            _logger = logger;
            _keyValuePersistenceService = keyValuePersistenceService;
            _getEASLock = new SemaphoreSlim(1, 1);
        }

        public async Task<IList<EasCsvRecord>> ProvideData()
        {
            List<EasCsvRecord> easRecords = null;

            await _getEASLock.WaitAsync(_cancellationToken);
            try
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    _keyValuePersistenceService.GetAsync(_jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename].ToString(), memoryStream, _cancellationToken).GetAwaiter().GetResult();
                    memoryStream.Position = 0;
                    using (StreamReader fileReader = new StreamReader(memoryStream))
                    {
                        var csv = new CsvReader(fileReader);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.RegisterClassMap<EasCsvRecordMapper>();
                        easRecords = csv.GetRecords<EasCsvRecord>().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to retrieve and deserialize EAS from storage, key: {JobContextMessageKey.Filename}", ex);
            }
            finally
            {
                _getEASLock.Release();
            }

            return easRecords;
        }

        public async Task<StreamReader> Provide(EasFileInfo easFileInfo, CancellationToken cancellationToken)
        {
            StreamReader streamReader = null;
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                await _keyValuePersistenceService.GetAsync(easFileInfo.FileName, memoryStream, _cancellationToken);
                memoryStream.Position = 0;
                streamReader = new StreamReader(memoryStream);
                return streamReader;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get EAS file from storage, key: {easFileInfo.FileName}", ex);
            }

            return streamReader;
        }
    }
}
