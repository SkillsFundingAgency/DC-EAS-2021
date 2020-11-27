using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Service;
using ESFA.DC.EAS.Stateless.Context;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Logging.Interfaces;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace ESFA.DC.EAS.Stateless
{
    public class JobContextMessageHandler : IMessageHandler<JobContextMessage>
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly StatelessServiceContext _context;
        private readonly ILogger _logger;

        public JobContextMessageHandler(ILifetimeScope lifetimeScope, StatelessServiceContext context)
        {
            _lifetimeScope = lifetimeScope;
            _context = context;
        }

        public async Task<bool> HandleAsync(JobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            try
            {
                var easContext = new EasJobContext(jobContextMessage);

                using (var childLifeTimeScope = _lifetimeScope.BeginLifetimeScope())
                {
                    var executionContext = (ExecutionContext)childLifeTimeScope.Resolve<IExecutionContext>();
                    executionContext.JobId = jobContextMessage.JobId.ToString();
                    var logger = childLifeTimeScope.Resolve<ILogger>();

                    var easServiceTasks = childLifeTimeScope.Resolve<IEnumerable<IEasServiceTask>>();
                    var serviceTasks = easServiceTasks.ToList();
                    var tasks = serviceTasks.Where(t => easContext.Tasks.Contains(t.TaskName)).ToList();

                    logger.LogDebug("Started EAS Service");
                    var entryPoint = childLifeTimeScope.Resolve<EntryPoint>();
                    var result = false;
                    try
                    {
                        logger.LogDebug($"Handling EAS - Message Tasks : {string.Join(", ", easContext.Tasks)} - EAS Service Tasks found in Registry : {string.Join(", ", serviceTasks.Select(t => t.TaskName))}");
                        result = await entryPoint.CallbackAsync(easContext, cancellationToken, tasks);
                    }
                    catch (OutOfMemoryException oom)
                    {
                        Environment.FailFast("EAS Service Out of memory", oom);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message, ex);
                    }

                    logger.LogDebug("Completed EAS Service");
                    return result;
                }
            }
            catch (OutOfMemoryException oom)
            {
                Environment.FailFast("EAS Service Out of memory", oom);
                throw;
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceMessage(_context, "Exception-{0}", ex.ToString());
                throw;
            }
        }
    }
}
