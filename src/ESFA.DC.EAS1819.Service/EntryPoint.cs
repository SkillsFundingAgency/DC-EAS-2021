using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Interface.Reports;
using ESFA.DC.EAS1819.Interface.Validation;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS1819.Service
{
    public class EntryPoint
    {
        private readonly ILogger _logger;
        private readonly IEASDataProviderService _easDataProviderService;
        private readonly IValidationService _validationService;
        private readonly IReportingController _reportingController;
        private readonly IFileHelper _fileHelper;

        public EntryPoint(
            ILogger logger,
            IEASDataProviderService easDataProviderService,
            IValidationService validationService,
            IReportingController reportingController,
            IFileHelper fileHelper)
        {
            _logger = logger;
            _easDataProviderService = easDataProviderService;
            _validationService = validationService;
            _reportingController = reportingController;
            _fileHelper = fileHelper;
        }

        public async Task<bool> CallbackAsync(IJobContextMessage jobContextMessage, CancellationToken cancellationToken, IList<IEasServiceTask> easServiceTasks)
       {
            _logger.LogInfo("EAS callback invoked");

            var jobContextMessageTasks = jobContextMessage.Topics[jobContextMessage.TopicPointer].Tasks;
            if (!jobContextMessageTasks.Any())
            {
                _logger.LogInfo("EAS. No tasks to run.");
                return true;
            }

            var fileInfo = _fileHelper.GetEASFileInfo(jobContextMessage);
            foreach (var task in easServiceTasks)
            {
                _logger.LogInfo($"EAS Service Task : {task.TaskName} Starting");
                await task.ExecuteAsync(jobContextMessage, fileInfo, cancellationToken);
                _logger.LogInfo($"EAS Service Task : {task.TaskName} Finished");
            }

            return true;
        }
    }
}
