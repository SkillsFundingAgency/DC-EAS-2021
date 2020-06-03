using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Constants;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Interface.Validation;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS.Service.Tasks
{
    public class ValidationTask : IEasServiceTask
    {
        private readonly IFileValidationService _fileValidationService;
        private readonly IValidationService _validationService;
        private readonly IFileDataCacheService _fileDataCacheService;
        private readonly IReportingController _reportingController;
        private readonly ILogger _logger;

        public ValidationTask(
            IFileValidationService fileValidationService,
            IValidationService validationService,
            IFileDataCacheService fileDataCacheService,
            IReportingController reportingController,
            ILogger logger)
        {
            _fileValidationService = fileValidationService;
            _validationService = validationService;
            _fileDataCacheService = fileDataCacheService;
            _reportingController = reportingController;
            _logger = logger;
        }

        public string TaskName => TaskNameConstants.ValidationTaskName;

        public async Task ExecuteAsync(IEasJobContext easJobContext, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Validate Task is called.");

            try
            {
                var fileValidationErrors = await _fileValidationService.ValidateFile(easJobContext, cancellationToken);

                if (fileValidationErrors.Any())
                {
                    return;
                }

                cancellationToken.ThrowIfCancellationRequested();

                await _validationService.ValidateDataAsync(easJobContext, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to validate data", ex);
                throw;
            }
        }
    }
}
