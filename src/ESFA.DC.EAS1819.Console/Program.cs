using Autofac;
using Autofac.Features.AttributeFilters;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS1819.DataService;
using ESFA.DC.EAS1819.DataService.FCS;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.DataService.Interface.FCS;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Interface.Reports;
using ESFA.DC.EAS1819.Interface.Validation;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.ReportingService;
using ESFA.DC.EAS1819.ReportingService.Reports;
using ESFA.DC.EAS1819.Service;
using ESFA.DC.EAS1819.Service.Helpers;
using ESFA.DC.EAS1819.Service.Import;
using ESFA.DC.EAS1819.Service.Providers;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;
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

            //FcsContext _fcsContext = new FcsContext("data source=(local);initial catalog=fcs;integrated security=True;multipleactiveresultsets=True;Connect Timeout=90");
            //var contractAllocations = _fcsContext.ContractAllocations.Where(x => x.Contract.Contractor.Ukprn == 10000421).Select(x => new { x.FundingStreamCode, x.StartDate, x.EndDate })
            //    .ToList();

            //"Topics": [
            //{
            //    "SubscriptionName": "process",
            //    "Tasks": [
            //    {
            //        "Tasks": [
            //        "Eas"
            //            ],
            //        "SupportsParallelExecution": false
            //    }
            //    ]
            //}
            //],

            var azureStorageConfig = new AzureStorageConfig(ConfigurationManager.AppSettings["AzureBlobConnectionString"], ConfigurationManager.AppSettings["AzureContainerReference"]);

            IJobContextMessage jobContextMessage = new JobContextMessage()
            {
                JobId = 100,
                KeyValuePairs = new Dictionary<string, object>()
                {
                    {"Filename" ,"EASDATA-10000421-20180912-144437.csv"}
                },
                SubmissionDateTimeUtc = DateTime.UtcNow,
                TopicPointer = 0,
                Topics = new List<ITopicItem>()
                {
                    new TopicItem() { SubscriptionName = "Process",
                                            Tasks = 
                                                new List<ITaskItem>()
                                                {
                                                    new TaskItem()
                                                    {
                                                        Tasks = new List<string>() { "Eas" }
                                                    }
                                                } }
                }
            };

            var azureStorageKeyValuePersistenceService = new AzureStorageKeyValuePersistenceService(azureStorageConfig);


            var _builder = new ContainerBuilder();
            Register.RegisterTypes(_builder);
            var _container = _builder.Build();

            var easFileDataProviderService =
                            new EASFileDataProviderService();
            //var easCsvRecords = easFileDataProviderService.ProvideAsync().Result;

            var fileInfo = new EasFileInfo()
            {
                FileName = "EASDATA-10000421-20180909-101010.csv",
                UKPRN = "10000421",
                DateTime = DateTime.UtcNow,
                FilePreparationDate = DateTime.UtcNow.AddHours(-2)
            };
            var submissionId = Guid.NewGuid();
            //ImportService importService = new ImportService(
            //    submissionId,
            //    _container.Resolve<IEasSubmissionService>(),
            //    _container.Resolve<IEasPaymentService>(),
            //    _container.Resolve<IValidationService>(),
            //    _container.Resolve<IReportingController>(),
            //    new SeriLogger(new ApplicationLoggerSettings(), new Logging.ExecutionContext(), null));
            //importService.ImportEasDataAsync(fileInfo, CancellationToken.None);

            EntryPoint entryPoint = new EntryPoint(
                new SeriLogger(new ApplicationLoggerSettings(), new Logging.ExecutionContext(), null),
                _container.Resolve<IEASDataProviderService>(),
                _container.Resolve<IValidationService>(),
                _container.Resolve<IReportingController>(),
                _container.Resolve<IFileHelper>());

            var easServiceTask = _container.Resolve<IEasServiceTask>();
            var result = entryPoint.CallbackAsync(jobContextMessage, CancellationToken.None, new List<IEasServiceTask>() { easServiceTask }).GetAwaiter().GetResult();
        }

        public class AzureStorageConfig : IAzureStorageKeyValuePersistenceServiceConfig
        {
            public AzureStorageConfig(string connectionString, string containerName)
            {
                AzureBlobConnectionString = connectionString;
                AzureContainerReference = containerName;
            }

            public string AzureBlobConnectionString { get; set; }

            public string AzureContainerReference { get; set; }

            public string ConnectionString => AzureBlobConnectionString;

            public string ContainerName => AzureContainerReference;
        }

        public class LoggerOptions
        {
            public string LoggerConnectionString { get; set; }
        }

        public static class Register
        {
            public static void RegisterTypes(ContainerBuilder builder)
            {
                var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];

                builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();
                builder.RegisterType<XmlSerializationService>().As<IXmlSerializationService>();

                //Logger
                //builder.RegisterInstance(new LoggerOptions()
                //{
                //    LoggerConnectionString = ConfigurationManager.AppSettings["EasdbConnectionString"]
                //}).As<ILoggerOptions>().SingleInstance();

                builder.Register(c =>
                        new AzureStorageConfig("", ""))
                    .As<IAzureStorageKeyValuePersistenceServiceConfig>().SingleInstance();


                builder.Register(c =>
                {
                    var loggerOptions = new LoggerOptions()
                    {
                        LoggerConnectionString = ConfigurationManager.AppSettings["EasdbConnectionString"]
                    };
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

                builder.RegisterType<Logging.ExecutionContext>().As<IExecutionContext>().InstancePerLifetimeScope();
                builder.RegisterType<SerilogLoggerFactory>().As<ISerilogLoggerFactory>().InstancePerLifetimeScope();
                builder.RegisterType<SeriLogger>().As<ILogger>().InstancePerLifetimeScope();


                builder.RegisterType<JobContextMessage>().As<IJobContextMessage>();
                builder.RegisterType<EasServiceTask>().As<IEasServiceTask>();
                //builder.RegisterType<EasAzureStorageDataProviderService>().As<IEASDataProviderService>();
                builder.RegisterType<EasValidationService>().As<IValidationService>();
                builder.RegisterType<CsvParser>().As<ICsvParser>();
                builder.Register(c =>
                {
                    var easdbContext = new ESFA.DC.EAS1819.EF.EasdbContext(connString);
                    easdbContext.Configuration.AutoDetectChangesEnabled = false;
                    return easdbContext;
                }).As<ESFA.DC.EAS1819.EF.Interface.IEasdbContext>().InstancePerDependency();

                builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
                builder.RegisterType<EasPaymentService>().As<IEasPaymentService>();
                builder.RegisterType<EasSubmissionService>().As<IEasSubmissionService>();
                builder.RegisterType<FCSDataService>().As<IFCSDataService>();
                builder.RegisterType<FundingLineContractTypeMappingDataService>().As<IFundingLineContractTypeMappingDataService>();
                builder.RegisterType<ValidationErrorService>().As<IValidationErrorService>();
                builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>();
                builder.RegisterType<ImportService>().As<IImportService>();

                builder.RegisterType<ViolationReport>().As<IValidationReport>();
                builder.RegisterType<FundingReport>().As<IModelReport>();
                builder.RegisterType<ValidationResultReport>().As<IValidationResultReport>();
                builder.RegisterType<ReportingController>().As<IReportingController>();
                builder.RegisterType<EASFileDataProviderService>().As<IEASDataProviderService>();
                builder.RegisterType<FCSDataService>().As<IFCSDataService>();
                builder.RegisterType<FundingLineContractTypeMappingDataService>().As<IFundingLineContractTypeMappingDataService>();
                builder.RegisterType<EasValidationService>().As<IValidationService>();
                builder.RegisterType<EntryPoint>().WithAttributeFiltering().InstancePerLifetimeScope();
                builder.RegisterType<FileHelper>().As<IFileHelper>();

                builder.RegisterType<AzureStorageKeyValuePersistenceService>()
                    .Keyed<IKeyValuePersistenceService>(PersistenceStorageKeys.AzureStorage)
                    .As<IStreamableKeyValuePersistenceService>()
                    .InstancePerLifetimeScope();

                builder.Register(c =>
                {
                    var fcsContext = new FcsContext("data source=(local);initial catalog=fcs;integrated security=True;multipleactiveresultsets=True;Connect Timeout=90");

                    fcsContext.Configuration.AutoDetectChangesEnabled = false;

                    return fcsContext;
                }).As<IFcsContext>().InstancePerDependency();

            }
        }
    }
}
