using ESFA.DC.EAS1819.Service.Providers;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Autofac;
using ESESFA.DC.EAS1819.DataService;
using ESFA.DC.EAS1819.DataService;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Service;
using ESFA.DC.EAS1819.Service.Interface;
using ESFA.DC.EAS1819.Service.Validation;

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

            //var azureStorageKeyValuePersistenceService = new AzureStorageKeyValuePersistenceService(azureStorageConfig);
            //var easAzureStorageDataProviderService = new EasAzureStorageDataProviderService(null,
            //    azureStorageKeyValuePersistenceService, jobContextMessage, new CancellationToken());
            //var azureStorageCsvRecords = easAzureStorageDataProviderService.Provide().Result;

            var _builder = new ContainerBuilder();
            Register.RegisterTypes(_builder);
           var _container = _builder.Build();

            var easFileDataProviderService = 
                            new EASFileDataProviderService(
                                @"C:\ESFA\DCT\EAS\EASDATA-12345678-20180924-100516.csv", 
                                _container.Resolve<IValidationService>(),
                                _container.Resolve<ICsvParser>(),
                                  new CancellationToken());
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

        public static class Register
        {
            public static void RegisterTypes(ContainerBuilder builder)
            {
                var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];

                //builder.RegisterType<LoggingService>().As<ILoggingService>();
                builder.RegisterType<EasValidationService>().As<IValidationService>();
                builder.RegisterType<CsvParser>().As<ICsvParser>();
                builder.RegisterType<EasdbContext>().WithParameter("nameOrConnectionString", connString);
                builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
                builder.RegisterType<EasPaymentService>().As<IEasPaymentService>();
                
            }
        }
    }
}
