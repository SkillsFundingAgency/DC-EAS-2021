using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Interface.Validation;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.Service.FileData;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS.Service.Tasks
{
    public class ValidationTask : IEasServiceTask
    {
        private readonly IEASDataProviderService _easDataProviderService;
        private readonly IValidationService _validationService;
        private readonly IFileDataCacheService _fileDataCacheService;
        private readonly IReportingController _reportingController;
        private readonly ILogger _logger;

        public ValidationTask(
            IEASDataProviderService easDataProviderService,
            IValidationService validationService,
            IFileDataCacheService fileDataCacheService,
            IReportingController reportingController,
            ILogger logger)
        {
            _easDataProviderService = easDataProviderService;
            _validationService = validationService;
            _fileDataCacheService = fileDataCacheService;
            _reportingController = reportingController;
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
                FileDataCache fileDataCache;
                try
                {
                    streamReader = await _easDataProviderService.ProvideAsync(fileInfo, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Azure service provider failed to return stream, key: {fileInfo.FileName}", ex);
                    throw;
                }

                using (streamReader)
                {
                    var validationErrorModel = _validationService.ValidateFile(streamReader, out easCsvRecords);
                    if (validationErrorModel.ErrorMessage != null)
                    {
                        await _validationService.LogValidationErrorsAsync(new List<ValidationErrorModel> { validationErrorModel }, fileInfo, cancellationToken);
                        await _reportingController.FileLevelErrorReportAsync(jobContextMessage, easCsvRecords, fileInfo, new List<ValidationErrorModel> { validationErrorModel }, cancellationToken);
                        fileDataCache = BuildFileDataCache(fileInfo, easCsvRecords, null, null, true);
                        await _fileDataCacheService.PopulateFileDataCacheAsync(fileDataCache, cancellationToken);
                        return;
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
                List<ValidationErrorModel> validationErrorModels = await _validationService.ValidateDataAsync(fileInfo, easCsvRecords.ToList(), cancellationToken);
                List<EasCsvRecord> validRecords = GetValidRows(easCsvRecords, validationErrorModels);
                fileDataCache = BuildFileDataCache(fileInfo, easCsvRecords, validRecords, validationErrorModels, false);
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
            List<ValidationErrorModel> validationErrorModels,
            bool failedFileValidation)
        {
            FileDataCache fileDataCache = new FileDataCache()
            {
                UkPrn = fileInfo.UKPRN,
                Filename = fileInfo.FileName,
                AllEasCsvRecords = easCsvRecords,
                ValidEasCsvRecords = validRecords,
                ValidationErrors = validationErrorModels,
                FailedFileValidation = failedFileValidation
            };
            return fileDataCache;
        }

        private List<EasCsvRecord> GetValidRows(IList<EasCsvRecord> data, List<ValidationErrorModel> validationErrorModels)
        {
            var easCsvRecords = data.Where(model => !validationErrorModels.Any(e => e.FundingLine == model.FundingLine
                                                                                    && e.AdjustmentType == model.AdjustmentType
                                                                                    && e.CalendarYear == model.CalendarYear
                                                                                    && e.CalendarMonth == model.CalendarMonth
                                                                                    && e.DevolvedAreaSoF == model.DevolvedAreaSourceOfFunding
                                                                                    && e.Severity == "E")).ToList();
            return easCsvRecords;
        }
    }
}
