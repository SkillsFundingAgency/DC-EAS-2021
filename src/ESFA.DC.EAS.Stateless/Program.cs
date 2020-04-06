using Autofac.Integration.ServiceFabric;
using System;
using System.Diagnostics;
using System.Threading;
using ESFA.DC.ServiceFabric.Common.Config;
using ESFA.DC.ServiceFabric.Common.Config.Interface;

namespace ESFA.DC.EAS.Stateless
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

                IServiceFabricConfigurationService serviceFabricConfigurationService = new ServiceFabricConfigurationService();

                var builder = DIComposition.BuildContainer(serviceFabricConfigurationService);

                builder.RegisterServiceFabricSupport();
                
                builder.RegisterStatelessService<ServiceFabric.Common.Stateless>("ESFA.DC.EAS2021.StatelessType");

                using (var container = builder.Build())
                {
                    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(ServiceFabric.Common.Stateless).Name);

                    // Prevents this host process from terminating so services keep running.
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e + Environment.NewLine + (e.InnerException?.ToString() ?? "No inner exception"));
                throw;
            }
        }
    }
}
