using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.ServiceFabric;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS1819.Stateless.Config;
using ESFA.DC.JobContextManager;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Mapping.Interface;
using ESFA.DC.ServiceFabric.Helpers;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ESFA.DC.EAS1819.Stateless
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                var builder = BuildContainer();

                builder.RegisterServiceFabricSupport();

                builder.RegisterStatelessService<Stateless>("ESFA.DC.EAS1819.StatelessType");

                using (var container = builder.Build())
                {
                    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(Stateless).Name);

                    // Prevents this host process from terminating so services keep running.
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static ContainerBuilder BuildContainer()
        {
            var easServiceConfiguration = GetEasServiceConfiguration();
           
            return new ContainerBuilder()
                .RegisterJobContextManagementServices();
                //.RegisterQueuesAndTopics(easServiceConfiguration)
                //.RegisterLogger(easServiceConfiguration)
                //.RegisterSerializers();
        }

        private static ContainerBuilder RegisterJobContextManagementServices(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<JobContextMessageHandler>().As<IMessageHandler<JobContextMessage>>();
            containerBuilder.RegisterType<JobContextManager<JobContextMessage>>().As<IJobContextManager<JobContextMessage>>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<DefaultJobContextMessageMapper<JobContextMessage>>().As<IMapper<JobContextMessage, JobContextMessage>>();
            containerBuilder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>();

            return containerBuilder;
        }
        private static EasServiceConfiguration GetEasServiceConfiguration()
        {
            var configHelper = new ConfigurationHelper();

            return configHelper.GetSectionValues<EasServiceConfiguration>("EasServiceConfiguration");
        }
    }
}
