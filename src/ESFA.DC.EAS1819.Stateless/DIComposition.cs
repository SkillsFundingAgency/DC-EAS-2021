﻿using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Features.AttributeFilters;
using ESFA.DC.Auditing.Interface;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS1819.DataService;
using ESFA.DC.EAS1819.DataService.FCS;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.DataService.Interface.FCS;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.EF.Interface;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Interface.FileData;
using ESFA.DC.EAS1819.Interface.Reports;
using ESFA.DC.EAS1819.Interface.Validation;
using ESFA.DC.EAS1819.ReportingService;
using ESFA.DC.EAS1819.ReportingService.Reports;
using ESFA.DC.EAS1819.Service;
using ESFA.DC.EAS1819.Service.FileData;
using ESFA.DC.EAS1819.Service.Helpers;
using ESFA.DC.EAS1819.Service.Import;
using ESFA.DC.EAS1819.Service.Providers;
using ESFA.DC.EAS1819.Service.Tasks;
using ESFA.DC.EAS1819.Stateless.Config;
using ESFA.DC.EAS1819.Stateless.Config.Interfaces;
using ESFA.DC.EAS1819.ValidationService;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.Dictionary;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Mapping.Interface;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;
using ESFA.DC.ServiceFabric.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.EAS1819.Stateless
{
    public static class DIComposition
    {
        public static ContainerBuilder BuildContainer(IConfigurationHelper configHelper)
        {
            var easServiceConfiguration = configHelper.GetSectionValues<EasServiceConfiguration>("EasServiceConfiguration");
            var fcsServiceConfiguration = configHelper.GetSectionValues<FcsServiceConfiguration>("FcsServiceConfiguration");
            var container = new ContainerBuilder()
                .RegisterAzureStorage(easServiceConfiguration)
                .RegisterJobContextManagementServices()
                .RegisterQueuesAndTopics(easServiceConfiguration)
                .RegisterLogger(easServiceConfiguration)
                .RegisterSerializers()
                .RegisterEasServices(easServiceConfiguration)
                .RegisterFcsServices(fcsServiceConfiguration);

            container.RegisterInstance(easServiceConfiguration).As<IEasServiceConfiguration>();

            return container;
        }

        private static ContainerBuilder RegisterSerializers(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();
            containerBuilder.RegisterType<XmlSerializationService>().As<IXmlSerializationService>();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterLogger(this ContainerBuilder containerBuilder, EasServiceConfiguration easServiceConfiguration)
        {
            containerBuilder.RegisterInstance(new LoggerOptions
            {
                LoggerConnectionString = easServiceConfiguration.LoggerConnectionString
            }).As<ILoggerOptions>().SingleInstance();

            containerBuilder.Register(c =>
            {
                var loggerOptions = c.Resolve<ILoggerOptions>();
                return new ApplicationLoggerSettings
                {
                    ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>
                    {
                        new MsSqlServerApplicationLoggerOutputSettings
                        {
                            MinimumLogLevel = LogLevel.Verbose,
                            ConnectionString = loggerOptions.LoggerConnectionString
                        },
                        new ConsoleApplicationLoggerOutputSettings
                        {
                            MinimumLogLevel = LogLevel.Verbose
                        }
                    }
                };
            }).As<IApplicationLoggerSettings>().SingleInstance();

            containerBuilder.RegisterType<ExecutionContext>().As<IExecutionContext>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<SerilogLoggerFactory>().As<ISerilogLoggerFactory>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<SeriLogger>().As<ILogger>().InstancePerLifetimeScope();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterQueuesAndTopics(this ContainerBuilder containerBuilder, EasServiceConfiguration easServiceConfiguration)
        {
            var topicSubscriptionConfig = new TopicConfiguration(easServiceConfiguration.ServiceBusConnectionString, easServiceConfiguration.TopicName, easServiceConfiguration.SubscriptionName, 1, maximumCallbackTimeSpan: TimeSpan.FromMinutes(40));

            containerBuilder.Register(c =>
            {
              return new TopicSubscriptionSevice<JobContextDto>(
                    topicSubscriptionConfig,
                    c.Resolve<IJsonSerializationService>(),
                    c.Resolve<ILogger>());
            }).As<ITopicSubscriptionService<JobContextDto>>();

            containerBuilder.Register(c =>
            {
                var topicPublishService =
                    new TopicPublishService<JobContextDto>(
                        topicSubscriptionConfig,
                        c.Resolve<IJsonSerializationService>());
                return topicPublishService;
            }).As<ITopicPublishService<JobContextDto>>();

            containerBuilder.Register(c =>
            {
                var auditPublishConfig = new QueueConfiguration(easServiceConfiguration.ServiceBusConnectionString, easServiceConfiguration.AuditQueueName, 1);

                return new QueuePublishService<AuditingDto>(
                    auditPublishConfig,
                    c.Resolve<IJsonSerializationService>());
            }).As<IQueuePublishService<AuditingDto>>();

            containerBuilder.Register(c =>
            {
                var jobStatusPublishConfig = new QueueConfiguration(easServiceConfiguration.ServiceBusConnectionString, easServiceConfiguration.JobStatusQueueName, 1);

                return new QueuePublishService<JobStatusDto>(
                    jobStatusPublishConfig,
                    c.Resolve<IJsonSerializationService>());
            }).As<IQueuePublishService<JobStatusDto>>();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterJobContextManagementServices(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<JobContextMessageHandler>().As<IMessageHandler<JobContextMessage>>();
            containerBuilder.RegisterType<EntryPoint>().WithAttributeFiltering().InstancePerLifetimeScope();
            containerBuilder.RegisterType<JobContextManager<JobContextMessage>>().As<IJobContextManager<JobContextMessage>>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<DefaultJobContextMessageMapper<JobContextMessage>>().As<IMapper<JobContextMessage, JobContextMessage>>();
            containerBuilder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterEasServices(this ContainerBuilder containerBuilder, EasServiceConfiguration easServiceConfiguration)
        {
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
            containerBuilder.RegisterType<ImportService>().As<IImportService>();
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
                .Keyed<IKeyValuePersistenceService>(PersistenceStorageKeys.AzureStorage)
                .As<IStreamableKeyValuePersistenceService>()
                .InstancePerLifetimeScope();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterFcsServices(this ContainerBuilder containerBuilder, IFcsServiceConfiguration fcsServiceConfiguration)
        {
            containerBuilder.Register(c =>
            {
                var fcsContext = new FcsContext(fcsServiceConfiguration.FcsConnectionString);

                fcsContext.Configuration.AutoDetectChangesEnabled = false;

                return fcsContext;
            }).As<IFcsContext>().InstancePerDependency();

            return containerBuilder;
        }
    }
}
