using Autofac;
using ESFA.DC.CsvService;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Service;
using ESFA.DC.FileService;
using ESFA.DC.FileService.Config.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Dictionary;
using ESFA.DC.IO.Interfaces;

namespace ESFA.DC.EAS.Stateless.Modules
{
    public class IOModule : Module
    {
        private readonly IAzureStorageFileServiceConfiguration _azureStorageFileServiceConfig;

        public IOModule(IAzureStorageFileServiceConfiguration azureStorageFileServiceConfig)
        {
            _azureStorageFileServiceConfig = azureStorageFileServiceConfig;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterInstance(_azureStorageFileServiceConfig).As<IAzureStorageFileServiceConfiguration>();

            containerBuilder.RegisterType<AzureStorageFileService>().As<IFileService>();
            containerBuilder.RegisterType<CsvFileService>().As<ICsvFileService>();
            containerBuilder.RegisterType<DictionaryKeyValuePersistenceService>().As<IKeyValuePersistenceService>().SingleInstance();
            containerBuilder.RegisterType<AzureStorageKeyValuePersistenceService>().As<IStreamableKeyValuePersistenceService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<ZipService>().As<IZipService>();
        }
    }
}
