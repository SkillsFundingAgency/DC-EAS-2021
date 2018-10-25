using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Interface.FileData;
using ESFA.DC.EAS1819.Interface.Validation;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.Service.FileData;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS1819.Service.Tasks
{
    public class ValidationTask : IEasServiceTask
    {
        private readonly IEASDataProviderService _easDataProviderService;
        private readonly IValidationService _validationService;
        private readonly IFileDataCacheService _fileDataCacheService;
        private readonly ILogger _logger;

        public ValidationTask(
            IEASDataProviderService easDataProviderService,
            IValidationService validationService,
            IFileDataCacheService fileDataCacheService,
            ILogger logger)
        {
            _easDataProviderService = easDataProviderService;
            _validationService = validationService;
            _fileDataCacheService = fileDataCacheService;
            _logger = logger;
        }

        public string TaskName => "Validation";

        public async Task ExecuteAsync(IJobContextMessage jobContextMessage, EasFileInfo fileInfo, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Validate Task is called.");

            try
            {
               List<EasCsvRecord> easCsvRecords;
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
                        return;
                    }
                }

                var validationErrorModels = await _validationService.ValidateDataAsync(fileInfo, easCsvRecords.ToList(), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                var validRecords = GetValidRows(easCsvRecords, validationErrorModels);
                var fileDataCache = BuildFileDataCache(fileInfo, easCsvRecords, validRecords, validationErrorModels);
                await _fileDataCacheService.PopulateFileDataCacheAsync(fileDataCache, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to validate data", ex);
                throw;
            }
        }

        private FileDataCache BuildFileDataCache(
            EasFileInfo fileInfo,
            List<EasCsvRecord> easCsvRecords,
            List<EasCsvRecord> validRecords,
            List<ValidationErrorModel> validationErrorModels)
        {
            FileDataCache fileDataCache = new FileDataCache()
            {
                //JobId = fileInfo.JobId,
                UkPrn = fileInfo.UKPRN,
                Filename = fileInfo.FileName,
                //SubmissionDateTime = fileInfo.DateTime,
                AllEasCsvRecords = easCsvRecords,
                ValidEasCsvRecords = validRecords,
                ValidationErrors = validationErrorModels
            };
            return fileDataCache;
        }

        private List<EasCsvRecord> GetValidRows(IList<EasCsvRecord> data, List<ValidationErrorModel> validationErrorModels)
        {
            var easCsvRecords = data.Where(model => !validationErrorModels.Any(e => e.FundingLine == model.FundingLine
                                                                                    && e.AdjustmentType == model.AdjustmentType
                                                                                    && e.CalendarYear == model.CalendarYear
                                                                                    && e.CalendarMonth == model.CalendarMonth
                                                                                    && e.Value == model.Value)).ToList();
            return easCsvRecords;
        }
    }
}
