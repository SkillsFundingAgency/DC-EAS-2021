using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.Interface.Validation;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.Service.Import;
using ESFA.DC.EAS1819.Service.Interface;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS1819.Service
{
    public class EasServiceTask : IEasServiceTask
    {
        private readonly IEasSubmissionService _easSubmissionService;
        private readonly IEasPaymentService _easPaymentService;
        private readonly IEASDataProviderService _easDataProviderService;
        private readonly ICsvParser _csvParser;
        private readonly IValidationService _validationService;
        private readonly IImportService _importService;
        private readonly ILogger _logger;

        public EasServiceTask(
            IEasSubmissionService easSubmissionService,
            IEasPaymentService easPaymentService,
            IEASDataProviderService easDataProviderService,
            ICsvParser csvParser,
            IValidationService validationService,
            IImportService importService,
            ILogger logger)
        {
            _easSubmissionService = easSubmissionService;
            _easPaymentService = easPaymentService;
            _easDataProviderService = easDataProviderService;
            _csvParser = csvParser;
            _validationService = validationService;
            _importService = importService;
            _logger = logger;
        }

        public string TaskName => "Eas";

        public async Task ExecuteAsync(IJobContextMessage jobContextMessage, EasFileInfo fileInfo, IList<EasCsvRecord> easCsvRecords, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Eas Import Service Task is called.");

            try
            {
                await _importService.ImportEasDataAsync(fileInfo, easCsvRecords, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to Import EAS Data", ex);
                throw;
            }
        }
    }
}
