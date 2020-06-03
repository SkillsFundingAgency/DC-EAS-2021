using Autofac;
using ESFA.DC.BulkCopy;
using ESFA.DC.BulkCopy.Interfaces;
using ESFA.DC.EAS.DataService;
using ESFA.DC.EAS.DataService.FCS;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.DataService.Interface.FCS;
using ESFA.DC.EAS.DataService.Interface.Postcodes;
using ESFA.DC.EAS.DataService.Persist;
using ESFA.DC.EAS.DataService.Postcodes;

namespace ESFA.DC.EAS.Stateless.Modules
{
    public class DataServicesModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<EasPaymentService>().As<IEasPaymentService>();
            containerBuilder.RegisterType<EasSubmissionService>().As<IEasSubmissionService>();
            containerBuilder.RegisterType<FCSDataService>().As<IFCSDataService>();
            containerBuilder.RegisterType<PostcodesDataService>().As<IPostcodesDataService>();
            containerBuilder.RegisterType<FundingLineContractTypeMappingDataService>().As<IFundingLineContractTypeMappingDataService>();

            containerBuilder.RegisterType<ValidationErrorLoggerService>().As<IValidationErrorLoggerService>();
            containerBuilder.RegisterType<BulkInsert>().As<IBulkInsert>();
        }
    }
}
