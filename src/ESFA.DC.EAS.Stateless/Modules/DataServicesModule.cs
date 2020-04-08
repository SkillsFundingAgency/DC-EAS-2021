using Autofac;
using ESFA.DC.EAS.DataService;
using ESFA.DC.EAS.DataService.FCS;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.DataService.Interface.FCS;

namespace ESFA.DC.EAS.Stateless.Modules
{
    public class DataServicesModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<EasPaymentService>().As<IEasPaymentService>();
            containerBuilder.RegisterType<EasSubmissionService>().As<IEasSubmissionService>();
            containerBuilder.RegisterType<FCSDataService>().As<IFCSDataService>();
            containerBuilder.RegisterType<FundingLineContractTypeMappingDataService>().As<IFundingLineContractTypeMappingDataService>();
        }
    }
}
