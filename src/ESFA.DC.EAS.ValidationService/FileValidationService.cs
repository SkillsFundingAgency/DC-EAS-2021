using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Validation;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ValidationService.Mapper;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.EAS.Interface.Constants;
using ESFA.DC.FileService.Interface;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.DataService.Interface;

namespace ESFA.DC.EAS.ValidationService
{
    public class FileValidationService : IFileValidationService
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IValidationErrorLoggerService _validationErrorLoggerService;
        private readonly IReportingController _reportingController;
        private readonly IFileDataCacheService _fileDataCacheService;
        private readonly IFileService _fileService;
        private readonly ILogger _logger;

        public FileValidationService(
            IDateTimeProvider dateTimeProvider,
            IValidationErrorLoggerService validationErrorLoggerService,
            IReportingController reportingController,
            IFileDataCacheService fileDataCacheService,
            IFileService fileService,
            ILogger logger)
        {
            _dateTimeProvider = dateTimeProvider;
            _validationErrorLoggerService = validationErrorLoggerService;
            _reportingController = reportingController;
            _fileDataCacheService = fileDataCacheService;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<List<ValidationErrorModel>> ValidateFile(IEasJobContext easJobContext, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Starting File Level validation");

            var validationErrorModels = new List<ValidationErrorModel>();

            try
            {
                using (var stream = await _fileService.OpenReadStreamAsync(easJobContext.FileReference, easJobContext.Container, cancellationToken))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        using (var csv = new CsvReader(reader))
                        {
                            csv.Configuration.HasHeaderRecord = true;
                            csv.Configuration.IgnoreBlankLines = true;
                            csv.Configuration.TrimOptions = TrimOptions.Trim;
                            csv.Configuration.RegisterClassMap(new EasCsvRecordMapper());
                            csv.Read();
                            csv.ReadHeader();
                            csv.ValidateHeader(typeof(EasCsvRecord));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("File Level validation - Error(s) found.");
                validationErrorModels.Add(new ValidationErrorModel()
                {
                    Severity = SeverityConstants.Error,
                    RuleName = ErrorNameConstants.FileFormat_01,
                    ErrorMessage = ErrorMessageConstants.FileFormat_01
                });
            }

            if (validationErrorModels.Any())
            {
                var easCsvRecords = Enumerable.Empty<EasCsvRecord>();

                await _validationErrorLoggerService.LogValidationErrorsAsync(easJobContext, validationErrorModels, cancellationToken);
                await _reportingController.FileLevelErrorReportAsync(easJobContext, easCsvRecords, validationErrorModels, cancellationToken);

                var fileDataCache = _fileDataCacheService.BuildFileDataCache(easJobContext.Ukprn, easJobContext.FileReference, easCsvRecords, null, null, true);
                await _fileDataCacheService.PopulateFileDataCacheAsync(fileDataCache, cancellationToken);
            }

            _logger.LogInfo("Finished File Level validation");

            return validationErrorModels;
        }
    }
}
