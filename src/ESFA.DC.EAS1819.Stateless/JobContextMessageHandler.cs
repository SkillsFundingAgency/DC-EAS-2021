﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.EAS1819.Service;
using ESFA.DC.EAS1819.Service.Interface;
using ESFA.DC.EAS1819.Stateless.Config;
using ESFA.DC.EAS1819.Stateless.Config.Interfaces;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS1819.Stateless
{
    public class JobContextMessageHandler :  IMessageHandler<JobContextMessage>
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
                using (var childLifeTimeScope = _lifetimeScope
                    .BeginLifetimeScope(c =>
                    {
                        var easServiceConfiguration = _lifetimeScope.Resolve<IEasServiceConfiguration>();

                        c.RegisterInstance(new AzureStorageKeyValuePersistenceConfig(
                                easServiceConfiguration.AzureBlobConnectionString,
                                jobContextMessage.KeyValuePairs[JobContextMessageKey.Container].ToString()))
                            .As<IAzureStorageKeyValuePersistenceServiceConfig>();
                    }))
                {
                    var executionContext = (Logging.ExecutionContext)childLifeTimeScope.Resolve<IExecutionContext>();
                    executionContext.JobId = jobContextMessage.JobId.ToString();
                    var logger = childLifeTimeScope.Resolve<ILogger>();

                    var taskNames = this.GetTaskNamesForTopicFromMessage(jobContextMessage);
                    var easServiceTasks = childLifeTimeScope.Resolve<IEnumerable<IEasServiceTask>>();
                    var tasks = easServiceTasks.Where(t => taskNames.Contains(t.TaskName)).ToList();

                    logger.LogDebug("Started EAS Service");
                    var entryPoint = childLifeTimeScope.Resolve<EntryPoint>();
                    var result = false;
                    try
                    {
                        logger.LogDebug($"Handling EAS - Message Tasks : {string.Join(", ", taskNames)} - EAS Service Tasks found in Registry : {string.Join(", ", easServiceTasks.Select(t => t.TaskName))}");
                        result = await entryPoint.Callback(jobContextMessage, cancellationToken, tasks);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message, ex);
                    }

                    logger.LogDebug("Completed EAS Service");
                    return result;
                }
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceMessage(this._context, "Exception-{0}", ex.ToString());
                throw;
            }
        }

        private IEnumerable<string> GetTaskNamesForTopicFromMessage(JobContextMessage jobContextMessage)
        {
            return jobContextMessage
                .Topics[jobContextMessage.TopicPointer]
                .Tasks
                .SelectMany(t => t.Tasks);
        }
    }
}
