using ESFA.DC.EAS1819.Service.Providers;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace ESFA.DC.EAS1819.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
           
            var azureStorageConfig = new AzureStorageConfig
            {
                AzureBlobConnectionString = ConfigurationManager.AppSettings["AzureBlobConnectionString"],
                AzureContainerReference = ConfigurationManager.AppSettings["AzureContainerReference"]
            };

            IJobContextMessage jobContextMessage = new JobContextMessage()
            {
                JobId = 100,
                KeyValuePairs = new Dictionary<string, object>()
                {
                    {"Filename" ,"EAS-10033670-1819-20180912-144437-03.csv"}
                },
                SubmissionDateTimeUtc = DateTime.UtcNow,
                TopicPointer = 1,
                Topics = new ArraySegment<ITopicItem>()
            };

            var azureStorageKeyValuePersistenceService = new AzureStorageKeyValuePersistenceService(azureStorageConfig);
            var easAzureStorageDataProviderService = new EasAzureStorageDataProviderService(null,
                azureStorageKeyValuePersistenceService, jobContextMessage, new CancellationToken());
            var azureStorageCsvRecords = easAzureStorageDataProviderService.Provide().Result;

            var easFileDataProviderService = new EASFileDataProviderService(@"C:\ESFA\DCT\EAS\EAS-10033670-1819-20180912-144437-03.csv", new CancellationToken());
            var easCsvRecords = easFileDataProviderService.Provide().Result;

        }

        public class AzureStorageConfig : IAzureStorageKeyValuePersistenceServiceConfig
        {
            public string AzureBlobConnectionString { get; set; }

            public string AzureContainerReference { get; set; }

            public string ConnectionString => AzureBlobConnectionString;

            public string ContainerName => AzureContainerReference;
        }

        public class LoggerOptions
        {
            public string LoggerConnectionstring { get; set; }
        }
    }
}
