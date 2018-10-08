﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ESESFA.DC.EAS1819.DataService;
using ESFA.DC.Auditing.Interface;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS1819.DataService;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Service;
using ESFA.DC.EAS1819.Service.Import;
using ESFA.DC.EAS1819.Service.Interface;
using ESFA.DC.EAS1819.Service.Providers;
using ESFA.DC.EAS1819.Stateless.Config;
using ESFA.DC.EAS1819.Stateless.Config.Interfaces;
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
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;
using ESFA.DC.ServiceFabric.Helpers;
using ESFA.DC.ServiceFabric.Helpers.Interfaces;

namespace ESFA.DC.EAS1819.Stateless
{
    public static class DIComposition
    {
        public static ContainerBuilder BuildContainer(IConfigurationHelper configHelper)
        {
            var easServiceConfiguration = configHelper.GetSectionValues<EasServiceConfiguration>("EasServiceConfiguration"); ;

            var container = new ContainerBuilder()
                .RegisterJobContextManagementServices()
                .RegisterQueuesAndTopics(easServiceConfiguration)
                .RegisterLogger(easServiceConfiguration)
                .RegisterSerializers()
                .RegisterEasServices(easServiceConfiguration);

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
            containerBuilder.RegisterInstance(new LoggerOptions()
            {
                LoggerConnectionString = easServiceConfiguration.LoggerConnectionString
            }).As<ILoggerOptions>().SingleInstance();

            containerBuilder.Register(c =>
            {
                var loggerOptions = c.Resolve<ILoggerOptions>();
                return new ApplicationLoggerSettings
                {
                    ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>()
                    {
                        new MsSqlServerApplicationLoggerOutputSettings()
                        {
                            MinimumLogLevel = LogLevel.Verbose,
                            ConnectionString = loggerOptions.LoggerConnectionString
                        },
                        new ConsoleApplicationLoggerOutputSettings()
                        {
                            MinimumLogLevel = LogLevel.Verbose
                        }
                    }
                };
            }).As<IApplicationLoggerSettings>().SingleInstance();

            containerBuilder.RegisterType<Logging.ExecutionContext>().As<IExecutionContext>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<SerilogLoggerFactory>().As<ISerilogLoggerFactory>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<SeriLogger>().As<ILogger>().InstancePerLifetimeScope();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterQueuesAndTopics(this ContainerBuilder containerBuilder, EasServiceConfiguration easServiceConfiguration)
        {
            containerBuilder.Register(c =>
            {
                var topicSubscriptionConfig = new TopicConfiguration(easServiceConfiguration.ServiceBusConnectionString, easServiceConfiguration.TopicName, easServiceConfiguration.SubscriptionName, 1, maximumCallbackTimeSpan: TimeSpan.FromMinutes(40));

                return new TopicSubscriptionSevice<JobContextDto>(
                    topicSubscriptionConfig,
                    c.Resolve<IJsonSerializationService>(),
                    c.Resolve<ILogger>());
            }).As<ITopicSubscriptionService<JobContextDto>>();

            containerBuilder.RegisterType<TopicPublishServiceStub<JobContextDto>>().As<ITopicPublishService<JobContextDto>>();

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
            containerBuilder.RegisterType<JobContextManager<JobContextMessage>>().As<IJobContextManager<JobContextMessage>>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<DefaultJobContextMessageMapper<JobContextMessage>>().As<IMapper<JobContextMessage, JobContextMessage>>();
            containerBuilder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>();

            return containerBuilder;
        }

        private static ContainerBuilder RegisterEasServices(this ContainerBuilder containerBuilder, EasServiceConfiguration easServiceConfiguration)
        {
            containerBuilder.RegisterType<JobContextMessage>().As<IJobContextMessage>();

            containerBuilder.RegisterType<EasServiceTask>().As<IEasServiceTask>();

            containerBuilder.RegisterType<EASFileDataProviderService>().As<IEASDataProviderService>();

            
            containerBuilder.RegisterType<EasValidationService>().As<IValidationService>();
            containerBuilder.RegisterType<CsvParser>().As<ICsvParser>();
            containerBuilder.RegisterType<EasdbContext>().WithParameter("nameOrConnectionString", easServiceConfiguration.EasdbConnectionString);
            containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            containerBuilder.RegisterType<EasPaymentService>().As<IEasPaymentService>();
            containerBuilder.RegisterType<EasSubmissionService>().As<IEasSubmissionService>();
            containerBuilder.RegisterType<ValidationErrorService>().As<IValidationErrorService>();
            containerBuilder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>();
            containerBuilder.RegisterType<ImportService>().As<IImportService>();

            return containerBuilder;
        }


    }
}
