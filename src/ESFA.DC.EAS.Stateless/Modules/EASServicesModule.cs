using Autofac;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.Service.FileData;
using ESFA.DC.EAS.Service.Helpers;
using ESFA.DC.EAS.Service.Providers;
using ESFA.DC.EAS.Service.Tasks;

namespace ESFA.DC.EAS.Stateless.Modules
{
    public class EASServicesModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ValidationTask>().As<IEasServiceTask>();
            containerBuilder.RegisterType<StorageTask>().As<IEasServiceTask>();
            containerBuilder.RegisterType<ReportingTask>().As<IEasServiceTask>();
            containerBuilder.RegisterType<EasAzureStorageDataProviderService>().As<IEASDataProviderService>();
            containerBuilder.RegisterType<FileHelper>().As<IFileHelper>();
            containerBuilder.RegisterType<FileDataCache>().As<IFileDataCache>().SingleInstance();
            containerBuilder.RegisterType<FileDataCacheService>().As<IFileDataCacheService>().SingleInstance();
        }
    }
}
