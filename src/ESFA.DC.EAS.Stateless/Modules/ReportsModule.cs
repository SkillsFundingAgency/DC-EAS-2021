using Autofac;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.ReportingService;
using ESFA.DC.EAS.ReportingService.Reports;
using ESFA.DC.EAS.Service.Helpers;

namespace ESFA.DC.EAS.Stateless.Modules
{
    public class ReportsModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ViolationReport>().As<IValidationReport>();
            containerBuilder.RegisterType<FundingReport>().As<IModelReport>();
            containerBuilder.RegisterType<ValidationResultReport>().As<IValidationResultReport>();
            containerBuilder.RegisterType<ReportingController>().As<IReportingController>();
            containerBuilder.RegisterType<FileNameService>().As<IFileNameService>();
        }
    }
}
