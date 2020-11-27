using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Constants;
using ESFA.DC.EAS.Interface.Validation;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS.Service.Tasks
{
    public class ValidationTask : IEasServiceTask
    {
        private readonly IFileValidationService _fileValidationService;
        private readonly IValidationService _validationService;
        private readonly IValidationErrorRuleService _validationErrorRuleService;
        private readonly ILogger _logger;

        public ValidationTask(
            IFileValidationService fileValidationService,
            IValidationService validationService,
            IValidationErrorRuleService validationErrorRuleService,
            ILogger logger)
        {
            _fileValidationService = fileValidationService;
            _validationService = validationService;
            _validationErrorRuleService = validationErrorRuleService;
            _logger = logger;
        }

        public string TaskName => TaskNameConstants.ValidationTaskName;

        public async Task ExecuteAsync(IEasJobContext easJobContext, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Validate Task is called.");

            try
            {
                var validationErrorReferenceData = await _validationErrorRuleService.GetAllValidationErrorRules(cancellationToken);

                var fileValidationErrors = await _fileValidationService.ValidateFile(easJobContext, validationErrorReferenceData, cancellationToken);

                if (fileValidationErrors.Any())
                {
                    return;
                }

                cancellationToken.ThrowIfCancellationRequested();

                await _validationService.ValidateDataAsync(easJobContext, validationErrorReferenceData, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to validate data", ex);
                throw;
            }
        }
    }
}
