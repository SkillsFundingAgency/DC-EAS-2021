using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Service.Interface;

using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS1819.Service
{
    public class EasServiceTask : IEasServiceTask
    {
        private readonly ILogger _logger;

        public EasServiceTask(ILogger logger)
        {
            _logger = logger;
        }

        public string TaskName => "Eas";

        public Task ExecuteAsync(IJobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            _logger.LogError("Eas Service Task is called.");
            return Task.CompletedTask;
        }
    }
}
