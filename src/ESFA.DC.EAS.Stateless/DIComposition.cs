using Autofac;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.EAS2021.EF.Interface;
using ESFA.DC.EAS.Stateless.Config;
using ESFA.DC.EAS.Stateless.Modules;
using ESFA.DC.FileService.Config;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.ServiceFabric.Common.Config.Interface;
using ESFA.DC.ServiceFabric.Common.Modules;
using Microsoft.EntityFrameworkCore;
using ESFA.DC.ReferenceData.Postcodes.Model;
using ESFA.DC.ReferenceData.Postcodes.Model.Interface;
using ESFA.DC.EAS.Interface.Config;

namespace ESFA.DC.EAS.Stateless
{
    public static class DIComposition
    {
        public static ContainerBuilder BuildContainer(IServiceFabricConfigurationService serviceFabricConfigurationService)
        {
            var statelessServiceConfiguration = serviceFabricConfigurationService.GetConfigSectionAsStatelessServiceConfiguration();

            var easServiceConfiguration = serviceFabricConfigurationService.GetConfigSectionAs<EasServiceConfiguration>("EasServiceConfiguration");
            var fcsServiceConfiguration = serviceFabricConfigurationService.GetConfigSectionAs<FcsServiceConfiguration>("FcsServiceConfiguration");
            var postcodesServiceConfiguration = serviceFabricConfigurationService.GetConfigSectionAs<PostcodesServiceConfiguration>("PostcodesServiceConfiguration");

            var azureStorageFileServiceConfiguration = new AzureStorageFileServiceConfiguration()
            {
                ConnectionString = easServiceConfiguration.AzureBlobConnectionString,
            };
            
            var container = new ContainerBuilder();

            container.RegisterModule(new StatelessServiceModule(statelessServiceConfiguration));
            container.RegisterModule<SerializationModule>();
            container.RegisterModule(new IOModule(azureStorageFileServiceConfiguration));
            container.RegisterModule<EASBaseModule>();
            container.RegisterModule<EASServicesModule>();
            container.RegisterModule<DataServicesModule>();
            container.RegisterModule<ValidationModule>();
            container.RegisterModule<ReportsModule>();

            container.RegisterInstance(easServiceConfiguration).As<IEasServiceConfiguration>();

            container.Register(c =>
            {
                DbContextOptions<EasContext> options = new DbContextOptionsBuilder<EasContext>().UseSqlServer(easServiceConfiguration.EasdbConnectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
                EasContext easdbContext = new EasContext(options);
                return easdbContext;
            }).As<IEasdbContext>().InstancePerDependency();

            container.Register(c =>
            {
                DbContextOptions<FcsContext> options = new DbContextOptionsBuilder<FcsContext>().UseSqlServer(fcsServiceConfiguration.FcsConnectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
                var fcsContext = new FcsContext(options);
                return fcsContext;
            }).As<IFcsContext>().InstancePerDependency();

            container.Register(c =>
            {
                DbContextOptions<PostcodesContext> options = new DbContextOptionsBuilder<PostcodesContext>().UseSqlServer(postcodesServiceConfiguration.PostcodesConnectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
                var postcodesContext = new PostcodesContext(options);
                return postcodesContext;
            }).As<IPostcodesContext>().InstancePerDependency();

            return container;
        }
    }
}
