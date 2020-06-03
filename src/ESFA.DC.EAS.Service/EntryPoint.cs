using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Interface.Validation;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS.Service
{
    public class EntryPoint
    {
        private readonly ILogger _logger;
        private readonly IValidationService _validationService;
        private readonly IReportingController _reportingController;

        public EntryPoint(
            ILogger logger,
            IValidationService validationService,
            IReportingController reportingController)
        {
            _logger = logger;
            _validationService = validationService;
            _reportingController = reportingController;
        }

        public async Task<bool> CallbackAsync(IEasJobContext easJobContext, CancellationToken cancellationToken, IList<IEasServiceTask> easServiceTasks)
       {
            _logger.LogInfo("EAS callback invoked");

            if (!easJobContext.Tasks.Any())
            {
                _logger.LogInfo("EAS. No tasks to run.");
                return true;
            }

            foreach (var task in easServiceTasks)
            {
                _logger.LogInfo($"EAS Service Task : {task.TaskName} Starting");
                await task.ExecuteAsync(easJobContext, cancellationToken);
                _logger.LogInfo($"EAS Service Task : {task.TaskName} Finished");
            }

            return true;
        }
    }
}
