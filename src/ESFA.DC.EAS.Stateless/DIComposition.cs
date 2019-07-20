using Autofac;
using Autofac.Features.AttributeFilters;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.DataService;
using ESFA.DC.EAS.DataService.FCS;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.DataService.Interface.FCS;
using ESFA.DC.EAS1920.EF;
using ESFA.DC.EAS1920.EF.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Interface.Validation;
using ESFA.DC.EAS.ReportingService;
using ESFA.DC.EAS.ReportingService.Reports;
using ESFA.DC.EAS.Service;
using ESFA.DC.EAS.Service.FileData;
using ESFA.DC.EAS.Service.Helpers;
using ESFA.DC.EAS.Service.Providers;
using ESFA.DC.EAS.Service.Tasks;
using ESFA.DC.EAS.Stateless.Config;
using ESFA.DC.EAS.Stateless.Config.Interfaces;
using ESFA.DC.EAS.ValidationService;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.Dictionary;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContextManager;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Mapping.Interface;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.ServiceFabric.Common.Config.Interface;
using ESFA.DC.ServiceFabric.Common.Modules;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.EAS.Stateless
{
    public static class DIComposition
    {
        public static ContainerBuilder BuildContainer(IServiceFabricConfigurationService serviceFabricConfigurationService)
        {
            var statelessServiceConfiguration = serviceFabricConfigurationService.GetConfigSectionAsStatelessServiceConfiguration();

            var easServiceConfiguration = serviceFabricConfigurationService.GetConfigSectionAs<EasServiceConfiguration>("EasServiceConfiguration");
            var fcsServiceConfiguration = serviceFabricConfigurationService.GetConfigSectionAs<FcsServiceConfiguration>("FcsServiceConfiguration");
            var container = new ContainerBuilder()
                .RegisterAzureStorage(easServiceConfiguration)
                .RegisterJobContextManagementServices()
                .RegisterEasServices(easServiceConfiguration)
                .RegisterFcsServices(fcsServiceConfiguration);

            container.RegisterModule(new StatelessServiceModule(statelessServiceConfiguration));
            container.RegisterModule<SerializationModule>();

            container.RegisterInstance(easServiceConfiguration).As<IEasServiceConfiguration>();

            return container;
        }

        private static ContainerBuilder RegisterJobContextManagementServices(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<JobContextMessageHandler>().As<IMessageHandler<JobContextMessage>>();
            
            containerBuilder.RegisterType<DefaultJobContextMessageMapper<JobContextMessage>>().As<IMapper<JobContextMessage, JobContextMessage>>();
            

            return containerBuilder;
        }

        private static ContainerBuilder RegisterEasServices(this ContainerBuilder containerBuilder, EasServiceConfiguration easServiceConfiguration)
        {
            containerBuilder.RegisterType<EntryPoint>().WithAttributeFiltering().InstancePerLifetimeScope();
            containerBuilder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>();

            containerBuilder.RegisterType<JobContextMessage>().As<IJobContextMessage>();
            containerBuilder.RegisterType<ValidationTask>().As<IEasServiceTask>();
            containerBuilder.RegisterType<StorageTask>().As<IEasServiceTask>();
            containerBuilder.RegisterType<ReportingTask>().As<IEasServiceTask>();
            containerBuilder.RegisterType<EasAzureStorageDataProviderService>().As<IEASDataProviderService>();
            containerBuilder.RegisterType<EasValidationService>().As<IValidationService>();
            containerBuilder.RegisterType<CsvParser>().As<ICsvParser>();
            containerBuilder.Register(c =>
            {
                DbContextOptions<EasContext> options = new DbContextOptionsBuilder<EasContext>().UseSqlServer(easServiceConfiguration.EasdbConnectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
                EasContext easdbContext = new EasContext(options);
                return easdbContext;
            }).As<IEasdbContext>().InstancePerDependency();

            containerBuilder.RegisterType<EasPaymentService>().As<IEasPaymentService>();
            containerBuilder.RegisterType<EasSubmissionService>().As<IEasSubmissionService>();
            containerBuilder.RegisterType<FCSDataService>().As<IFCSDataService>();
            containerBuilder.RegisterType<FundingLineContractTypeMappingDataService>().As<IFundingLineContractTypeMappingDataService>();
            containerBuilder.RegisterType<ValidationErrorService>().As<IValidationErrorService>();
            containerBuilder.RegisterType<ValidationErrorRuleService>().As<IValidationErrorRuleService>();
            containerBuilder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>();
            containerBuilder.RegisterType<FileHelper>().As<IFileHelper>();
            containerBuilder.RegisterType<FileDataCache>().As<IFileDataCache>().SingleInstance();
            containerBuilder.RegisterType<FileDataCacheService>().As<IFileDataCacheService>().SingleInstance();
            containerBuilder.RegisterType<DictionaryKeyValuePersistenceService>().As<IKeyValuePersistenceService>().SingleInstance();
            containerBuilder.RegisterType<ViolationReport>().As<IValidationReport>();
            containerBuilder.RegisterType<FundingReport>().As<IModelReport>();
            containerBuilder.RegisterType<ValidationResultReport>().As<IValidationResultReport>();
            containerBuilder.RegisterType<ReportingController>().As<IReportingController>();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterAzureStorage(this ContainerBuilder containerBuilder, EasServiceConfiguration easServiceConfiguration)
        {
            // Following is registred in the childscope--> jobcontextMessageHandler
            //containerBuilder.Register(c =>
            //        new AzureStorageKeyValuePersistenceConfig(
            //            easServiceConfiguration.AzureBlobConnectionString,
            //            easServiceConfiguration.AzureBlobContainerName))
            //    .As<IAzureStorageKeyValuePersistenceServiceConfig>().SingleInstance();

            containerBuilder.RegisterType<AzureStorageKeyValuePersistenceService>()
                //.Keyed<IKeyValuePersistenceService>(PersistenceStorageKeys.AzureStorage)
                .As<IStreamableKeyValuePersistenceService>()
                .InstancePerLifetimeScope();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterFcsServices(this ContainerBuilder containerBuilder, IFcsServiceConfiguration fcsServiceConfiguration)
        {
            containerBuilder.Register(c =>
            {
                DbContextOptions<FcsContext> options = new DbContextOptionsBuilder<FcsContext>().UseSqlServer(fcsServiceConfiguration.FcsConnectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
                var fcsContext = new FcsContext(options);
                return fcsContext;
            }).As<IFcsContext>().InstancePerDependency();

            return containerBuilder;
        }
    }
}
