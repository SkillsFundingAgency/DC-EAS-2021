using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.AttributeFilters;
using ESFA.DC.BulkCopy;
using ESFA.DC.BulkCopy.Interfaces;
using ESFA.DC.CsvService;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.Acceptance.Test.Stubs;
using ESFA.DC.EAS.DataService;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.DataService.Interface.FCS;
using ESFA.DC.EAS.DataService.Interface.Postcodes;
using ESFA.DC.EAS.DataService.Persist;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Config;
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
using ESFA.DC.EAS.ValidationService;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.EAS2021.EF.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.IO.Dictionary;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.ReferenceData.Postcodes.Model;
using ESFA.DC.ReferenceData.Postcodes.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ESFA.DC.EAS.Acceptance.Test
{
    public partial class EasAcceptanceTests
    {
        public static class DIComposition
        {
            public static void RegisterTypes(ContainerBuilder builder)
            {
                Mock<IFCSDataService> fcsDataServiceMock = new Mock<IFCSDataService>();
                fcsDataServiceMock.Setup(x => x.GetContractsForProviderAsync(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync(
                    new List<ContractAllocation>()
                    {
                        new ContractAllocation { FundingStreamPeriodCode = "APPS2021", StartDate = new DateTime(2020, 01, 01), EndDate = new DateTime(2021, 12, 31) },
                        new ContractAllocation { FundingStreamPeriodCode = "LEVY1799", StartDate = new DateTime(2019, 01, 01), EndDate = new DateTime(2020, 12, 31) },
                        new ContractAllocation { FundingStreamPeriodCode = "NONLEVY2020", StartDate = new DateTime(2019, 01, 01), EndDate = new DateTime(2020, 12, 31) },
                        new ContractAllocation { FundingStreamPeriodCode = "16-18NLAP2019", StartDate = new DateTime(2019, 01, 01), EndDate = new DateTime(2020, 12, 31) },
                        new ContractAllocation { FundingStreamPeriodCode = "ANLAP2019", StartDate = new DateTime(2019, 01, 01), EndDate = new DateTime(2020, 12, 31) },
                        new ContractAllocation { FundingStreamPeriodCode = "16-18TRN2021", StartDate = new DateTime(2020, 01, 01), EndDate = new DateTime(2021, 12, 31) },
                        new ContractAllocation { FundingStreamPeriodCode = "AEBC-ASCL2021", StartDate = new DateTime(2020, 01, 01), EndDate = new DateTime(2021, 12, 31) },
                        new ContractAllocation { FundingStreamPeriodCode = "AEBC-19TRN2021", StartDate = new DateTime(2020, 01, 01), EndDate = new DateTime(2021, 12, 31) },
                        new ContractAllocation { FundingStreamPeriodCode = "AEB-AS2021", StartDate = new DateTime(2020, 01, 01), EndDate = new DateTime(2021, 12, 31) },
                        new ContractAllocation { FundingStreamPeriodCode = "AEB-19TRN2021", StartDate = new DateTime(2020, 01, 01), EndDate = new DateTime(2021, 12, 31) },
                        new ContractAllocation { FundingStreamPeriodCode = "ALLB2021", StartDate = new DateTime(2020, 01, 01), EndDate = null },
                        new ContractAllocation { FundingStreamPeriodCode = "ALLBC2021", StartDate = new DateTime(2020, 01, 01) }
                    });
                fcsDataServiceMock.Setup(x => x.GetDevolvedContractsForProviderAsync(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync(
                   new Dictionary<string, IEnumerable<DevolvedContract>>()
                   {
                        {
                            "WMCA",  new List<DevolvedContract>
                            {
                                new DevolvedContract { McaglashortCode = "WMCA", EffectiveFrom = new DateTime(2020, 01, 01), EffectiveTo = new DateTime(2021, 07, 31) }
                            } as IEnumerable<DevolvedContract>
                        }
                   } as IReadOnlyDictionary<string, IEnumerable<DevolvedContract>>);

                Mock<IPostcodesDataService> postcodesDataServiceMock = new Mock<IPostcodesDataService>();
                postcodesDataServiceMock.Setup(x => x.GetMcaShortCodesForSofCodesAsync(It.IsAny<IEnumerable<int>>(), CancellationToken.None)).ReturnsAsync(
                    new Dictionary<int, string>()
                    {
                        { 110, "GMCA" },
                        { 111, "LCRCA" },
                        { 112, "WMCA" },
                        { 113, "WECA" },
                        { 114, "TVCA" },
                        { 115, "CPCA" },
                        { 116, "London" },
                        { 117, "NTCA" },
                    } as IReadOnlyDictionary<int, string>);

                Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
                dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 11, 01, 10, 10, 10));
                dateTimeProviderMock.Setup(x => x.ConvertUkToUtc(It.IsAny<DateTime>())).Returns<DateTime>(d => d);
                dateTimeProviderMock.Setup(x => x.ConvertUkToUtc(It.IsAny<string>(), It.IsAny<string>())).Returns(new DateTime(2020, 11, 01, 10, 10, 10));

                Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
                var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];

                builder.RegisterInstance(fcsDataServiceMock.Object).As<IFCSDataService>();
                builder.RegisterInstance(postcodesDataServiceMock.Object).As<IPostcodesDataService>();
                builder.RegisterInstance(dateTimeProviderMock.Object).As<IDateTimeProvider>();
                builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();
                builder.RegisterType<XmlSerializationService>().As<IXmlSerializationService>();
                //builder.Register(c =>
                //        new AzureStorageConfig("DefaultEndpointsProtocol=https;AccountName=test;AccountKey=test;EndpointSuffix=core.windows.net", "test"))
                //    .As<IAzureStorageKeyValuePersistenceServiceConfig>().SingleInstance();
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

                builder.RegisterType<ValidationTask>().As<IEasServiceTask>();
                builder.RegisterType<StorageTask>().As<IEasServiceTask>();
                builder.RegisterType<ReportingTask>().As<IEasServiceTask>();

                builder.RegisterType<EasValidationService>().As<IValidationService>();
                builder.RegisterType<FileValidationService>().As<IFileValidationService>();
                builder.RegisterType<ValidationErrorLoggerService>().As<IValidationErrorLoggerService>();
                builder.RegisterType<EASFileDataProviderService>().As<IEASFileDataProviderService>();
                builder.Register(c =>
                {
                    DbContextOptions<EasContext> options = new DbContextOptionsBuilder<EasContext>().UseSqlServer(connString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
                    EasContext easdbContext = new EasContext(options);
                    return easdbContext;
                }).As<IEasdbContext>().InstancePerDependency();

                builder.Register(c => new EasConfig
                {
                    EasdbConnectionString = connString
                }).As<IEasServiceConfiguration>();

                builder.RegisterType<EasPaymentService>().As<IEasPaymentService>();
                builder.RegisterType<EasSubmissionService>().As<IEasSubmissionService>();
                builder.RegisterType<FundingLineContractTypeMappingDataService>().As<IFundingLineContractTypeMappingDataService>();
                builder.RegisterType<ValidationErrorRetrievalService>().As<IValidationErrorRetrievalService>();
                builder.RegisterType<ValidationErrorRuleService>().As<IValidationErrorRuleService>();
                builder.RegisterType<FileDataCache>().As<IFileDataCache>().SingleInstance();
                builder.RegisterType<FileDataCacheService>().As<IFileDataCacheService>();
                builder.RegisterType<DictionaryKeyValuePersistenceService>().As<IKeyValuePersistenceService>().SingleInstance();
                builder.RegisterType<ViolationReport>().As<IValidationReport>();
                builder.RegisterType<FundingReport>().As<IModelReport>();
                builder.RegisterType<ValidationResultReport>().As<IValidationResultReport>();
                builder.RegisterType<ReportingController>().As<IReportingController>();
                builder.RegisterType<FundingLineContractTypeMappingDataService>().As<IFundingLineContractTypeMappingDataService>();
                builder.RegisterType<EasValidationService>().As<IValidationService>();
                builder.RegisterType<EntryPoint>().WithAttributeFiltering().InstancePerLifetimeScope();
                builder.RegisterType<FileNameService>().As<IFileNameService>();
                builder.RegisterType<CsvFileService>().As<ICsvFileService>();
                builder.RegisterType<ZipService>().As<IZipService>();
                builder.RegisterType<FileServiceStub>().As<IFileService>();
                builder.RegisterType<BulkInsert>().As<IBulkInsert>();

                builder.Register(c =>
                {
                    var fcsContext = new FcsContext();

//                    fcsContext.Configuration.AutoDetectChangesEnabled = false;

                    return fcsContext;
                }).As<IFcsContext>().InstancePerDependency();

                builder.Register(c =>
                {
                    var postcodesContext = new PostcodesContext();
                    return postcodesContext;
                }).As<IPostcodesContext>().InstancePerDependency();
            }
        }
    }
}
