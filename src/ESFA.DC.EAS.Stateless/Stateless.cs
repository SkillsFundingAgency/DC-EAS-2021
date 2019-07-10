using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ESFA.DC.EAS1920.Stateless
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class Stateless : StatelessService
    {
        private readonly IJobContextManager<JobContextMessage> _jobContextManager;
        private readonly ILogger _logger;

        public Stateless(StatelessServiceContext context, IJobContextManager<JobContextMessage> jobContextManager, ILogger logger)
            : base(context)
        {
            _jobContextManager = jobContextManager;
            _logger = logger;
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            bool initialised = false;
            try
            {
                _logger.LogInfo("EAS - Stateless Service Start");

                _jobContextManager.OpenAsync(cancellationToken);
                initialised = true;
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (Exception exception) when (!(exception is TaskCanceledException))
            {
                // Ignore, as an exception is only really thrown on cancellation of the token.
                _logger.LogError("EAS - Stateless Service Exception", exception);
            }
            finally
            {
                if (initialised)
                {
                    _logger.LogInfo("EAS - Stateless Service End");
                    await _jobContextManager.CloseAsync();
                }
            }
        }
    }
}
