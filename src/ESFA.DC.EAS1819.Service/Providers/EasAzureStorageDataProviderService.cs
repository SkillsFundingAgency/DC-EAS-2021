using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using CsvHelper;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.Service.Mapper;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS1819.Service.Providers
{
    public class EasAzureStorageDataProviderService : IEASDataProviderService
    {
        private readonly ILogger _logger;
        private readonly IStreamableKeyValuePersistenceService _keyValuePersistenceService;
        private readonly SemaphoreSlim _getEasLock;

        public EasAzureStorageDataProviderService(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.AzureStorage)]IStreamableKeyValuePersistenceService keyValuePersistenceService)
        {
            _logger = logger;
            _keyValuePersistenceService = keyValuePersistenceService;
            _getEasLock = new SemaphoreSlim(1, 1);
        }

        public async Task<IList<EasCsvRecord>> ProvideData(IJobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            List<EasCsvRecord> easRecords = null;

            await _getEasLock.WaitAsync(cancellationToken);
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await _keyValuePersistenceService.GetAsync(jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename].ToString(), memoryStream, cancellationToken);
                    memoryStream.Position = 0;
                    using (StreamReader fileReader = new StreamReader(memoryStream, Encoding.UTF8, true, 1024, true))
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
                _getEasLock.Release();
            }

            return easRecords;
        }

        public async Task<StreamReader> ProvideAsync(EasFileInfo easFileInfo, CancellationToken cancellationToken)
        {
            StreamReader streamReader = null;
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                await _keyValuePersistenceService.GetAsync(easFileInfo.FileName, memoryStream, cancellationToken);
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
