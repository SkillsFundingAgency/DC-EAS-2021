using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Interface.Reports;
using ESFA.DC.EAS1819.Interface.Validation;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model;
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

        public EntryPoint(ILogger logger, IEASDataProviderService easDataProviderService, IValidationService validationService, IReportingController reportingController)
        {
            _logger = logger;
            _easDataProviderService = easDataProviderService;
            _validationService = validationService;
            _reportingController = reportingController;
        }

        public async Task<bool> CallbackAsync(IJobContextMessage jobContextMessage, CancellationToken cancellationToken, List<IEasServiceTask> easServiceTasks)
        {
            _logger.LogInfo("EAS callback invoked");

            var jobContextMessageTasks = jobContextMessage.Topics[jobContextMessage.TopicPointer].Tasks;
            if (!jobContextMessageTasks.Any())
            {
                _logger.LogInfo("EAS. No tasks to run.");
                return true;
            }

            var fileInfo = BuildEasFileInfo(jobContextMessage);
            IList<EasCsvRecord> easCsvRecords;
            StreamReader streamReader;
            try
            {
                streamReader = await _easDataProviderService.ProvideAsync(fileInfo, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Azure service provider failed to return stream, key: {fileInfo.FileName}", ex);
                throw ex;
            }

            cancellationToken.ThrowIfCancellationRequested();

            using (streamReader)
            {
                var validationErrorModel = _validationService.ValidateFile(streamReader, out easCsvRecords);
                if (validationErrorModel.ErrorMessage != null)
                {
                    _validationService.LogValidationErrors(new List<ValidationErrorModel> { validationErrorModel }, fileInfo);
                    await _reportingController.FileLevelErrorReportAsync(null, fileInfo, new List<ValidationErrorModel> { validationErrorModel }, cancellationToken);
                    return false;
                }
            }

            foreach (var task in easServiceTasks)
            {
                _logger.LogInfo($"EAS Service Task : {task.TaskName} Starting");
                await task.ExecuteAsync(jobContextMessage, fileInfo, easCsvRecords, cancellationToken);
                _logger.LogInfo($"EAS Service Task : {task.TaskName} Finished");
            }

            return true;
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
