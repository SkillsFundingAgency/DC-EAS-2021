using ESFA.DC.EAS1819.Service.Providers;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Autofac;
using ESFA.DC.EAS1819.DataService;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS1819.DataService;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Interface.Reports;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.Service;
using ESFA.DC.EAS1819.Service.Import;
using ESFA.DC.EAS1819.Service.Interface;
using ESFA.DC.EAS1819.Service.Validation;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.ReferenceData.FCS.Model;

namespace ESFA.DC.EAS1819.Console
{
    public class Program
    {
        static void Main(string[] args)
        {

            FcsContext _fcsContext = new FcsContext("data source=(local);initial catalog=fcs;integrated security=True;multipleactiveresultsets=True;Connect Timeout=90");
            var contractAllocations = _fcsContext.ContractAllocations.Where(x => x.Contract.Contractor.Ukprn == 10000421).Select(x=> new { x.FundingStreamCode,x.StartDate, x.EndDate })
                .ToList();


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
                    {"Filename" ,"EASDATA-12345678-20180924-100516.csv"}
                },
                SubmissionDateTimeUtc = DateTime.UtcNow,
                TopicPointer = 1,
                Topics = new ArraySegment<ITopicItem>()
            };

            var azureStorageKeyValuePersistenceService = new AzureStorageKeyValuePersistenceService(azureStorageConfig);
           

            var _builder = new ContainerBuilder();
            Register.RegisterTypes(_builder);
            var _container = _builder.Build();

            var easFileDataProviderService =
                            new EASFileDataProviderService();
            //var easCsvRecords = easFileDataProviderService.Provide().Result;

            var fileInfo = new EasFileInfo()
            {
                FileName = "EAS-10033670-1819-20180912-144437-03.csv",
                UKPRN = "10033670",
                DateTime = DateTime.UtcNow,
                FilePreparationDate = DateTime.UtcNow.AddHours(-2)
            };
            var submissionId = Guid.NewGuid();
            ImportService importService = new ImportService(
                submissionId,
                _container.Resolve<IEasSubmissionService>(),
                _container.Resolve<IEasPaymentService>(),
                easFileDataProviderService,
                _container.Resolve<ICsvParser>(),
                _container.Resolve<IValidationService>(),
                _container.Resolve<IReportingController>(),
                azureStorageKeyValuePersistenceService);
            importService.ImportEasData(fileInfo, CancellationToken.None);
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
                builder.RegisterType<EasSubmissionService>().As<IEasSubmissionService>();
                builder.RegisterType<ValidationErrorService>().As<IValidationErrorService>();
                builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>();
                
            }
        }
    }
}
