using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Service.Interface;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS1819.Stateless
{
    public class JobContextMessageHandler :  IMessageHandler<JobContextMessage>
    {
        private readonly IEnumerable<IEasServiceTask> _easServiceTasks;
        private readonly ILogger _logger;

        public JobContextMessageHandler(IEnumerable<IEasServiceTask> easServiceTasks, ILogger logger)
        {
            _easServiceTasks = easServiceTasks;
            _logger = logger;
        }

        public async Task<bool> HandleAsync(JobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            try
            {
                var taskNames = GetTaskNamesForTopicFromMessage(jobContextMessage);

                var tasks = _easServiceTasks.Where(t => taskNames.Contains(t.TaskName)).ToList();

                _logger.LogInfo($"Handling EAS - Message Tasks : {string.Join(", ", taskNames)} - EAS Service Tasks found in Registry : {string.Join(", ", tasks.Select(t => t.TaskName))}");

                foreach (var task in tasks)
                {
                    _logger.LogInfo($"EAS Service Task : {task.TaskName} Starting");

                    await task.ExecuteAsync(jobContextMessage,cancellationToken);

                    _logger.LogInfo($"EAS Service Task : {task.TaskName} Finished");
                }
            }
            catch (Exception exception)
            {
                _logger.LogError("EAS Service Message Handler Failed", exception);
                throw;
            }

            return true;
        }

        public IEnumerable<string> GetTaskNamesForTopicFromMessage(JobContextMessage jobContextMessage)
        {
            return jobContextMessage
                .Topics[jobContextMessage.TopicPointer]
                .Tasks
                .SelectMany(t => t.Tasks);
        }
    }
}
