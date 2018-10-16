using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.DataService.Interface;
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

        public Task ExecuteAsync(IJobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Eas Service Task is called.");
            var fileInfo = BuildEasFileInfo(jobContextMessage);
            try
            {
                _importService.ImportEasData(fileInfo, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return Task.CompletedTask;
        }

        private EasFileInfo BuildEasFileInfo(IJobContextMessage jobContextMessage)
        {
            if (!jobContextMessage.KeyValuePairs.ContainsKey(JobContextMessageKey.Filename))
            {
                throw new ArgumentException($"{nameof(JobContextMessageKey.Filename)} is required");
            }

            var fileName = jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename].ToString();
            string[] fileNameParts = fileName.Substring(0, fileName.IndexOf('.') - 1).Split('-');

            if (fileNameParts.Length != 4)
            {
                throw new ArgumentException($"{nameof(JobContextMessageKey.Filename)} is invalid");
            }

            //if (!DateTime.TryParse(fileNameParts[3], out var preparationDateTime))
            //{
            //    throw new ArgumentException($"{nameof(JobContextMessageKey.Filename)} is invalid");
            //}

            var fileInfo = new EasFileInfo
            {
                JobId = jobContextMessage.JobId,
                FilePreparationDate = DateTime.UtcNow, // preparationDateTime
                FileName = fileName,
                DateTime = jobContextMessage.SubmissionDateTimeUtc,
                UKPRN = fileNameParts[1]
            };
            return fileInfo;
        }
    }
}
