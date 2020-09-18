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
using ESFA.DC.EAS.Service.Exceptions;

namespace ESFA.DC.EAS.ValidationService
{
    public class FileValidationService : IFileValidationService
    {
        private readonly IEnumerable<string> _easCSVHeaders = new List<string>
        {
            "FundingLine", "AdjustmentType", "CalendarYear", "CalendarMonth", "Value", "DevolvedAreaSourceOfFunding"
        };

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

            await ValidateHeader(easJobContext, validationErrorModels, cancellationToken);

            if (validationErrorModels.Any())
            {
                await GenerateErrorOutputsAsync(easJobContext, validationErrorModels, cancellationToken);

                return validationErrorModels;
            }

            await ValidateContent(easJobContext, validationErrorModels, cancellationToken);

            if (validationErrorModels.Any())
            {
                await GenerateErrorOutputsAsync(easJobContext, validationErrorModels, cancellationToken);

                return validationErrorModels;
            }

            _logger.LogInfo("Finished File Level validation");

            return validationErrorModels;
        }

        public async Task<ICollection<ValidationErrorModel>> ValidateHeader(IEasJobContext easJobContext, ICollection<ValidationErrorModel> errorModels, CancellationToken cancellationToken)
        {
            try
            {
                string[] header;

                using (var stream = await _fileService.OpenReadStreamAsync(easJobContext.FileReference,
                    easJobContext.Container, cancellationToken))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        using (var csvReader = new CsvReader(reader))
                        {
                            csvReader.Read();
                            csvReader.ReadHeader();

                            header = csvReader.Context.HeaderRecord;
                        }
                    }
                }

                if (_easCSVHeaders.Count() != header.Count())
                {
                    throw new InvalidCsvHeaderException();
                }

                foreach (var column in _easCSVHeaders)
                {
                    if (!header.Contains(column))
                    {
                        throw new InvalidCsvHeaderException();
                    }
                }
            }
            catch (InvalidCsvHeaderException ex)
            {
                errorModels.Add(BuildValidationError(ErrorNameConstants.FileFormat_01));
            }
            catch (Exception ex)
            {
                errorModels.Add(BuildValidationError(ErrorNameConstants.FileFormat_02));
            }

            return errorModels;
        }

        public async Task<ICollection<ValidationErrorModel>> ValidateContent(IEasJobContext easJobContext, ICollection<ValidationErrorModel> errorModels, CancellationToken cancellationToken)
        {
            try
            {
                using (var stream = await _fileService.OpenReadStreamAsync(easJobContext.FileReference, easJobContext.Container, cancellationToken))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        using (var csv = new CsvReader(reader))
                        {
                            csv.Configuration.HasHeaderRecord = true;
                            csv.Configuration.HeaderValidated = null;
                            csv.Configuration.IgnoreBlankLines = true;
                            csv.Configuration.TrimOptions = TrimOptions.Trim;
                            csv.Configuration.RegisterClassMap(new EasCsvRecordMapper());
                            //csv.Read();

                           // var f = csv.GetField(1);

                            var records = csv.GetRecords<EasCsvRecord>().ToList();
                            var t = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorModels.Add(BuildValidationError(ErrorNameConstants.FileFormat_02));
               // return errorModels;
            }

            return errorModels;
        }

        private async Task GenerateErrorOutputsAsync(IEasJobContext easJobContext, ICollection<ValidationErrorModel> errorModels, CancellationToken cancellationToken)
        {
            _logger.LogInfo("File Level validation - Error(s) found.");
            var easCsvRecords = Enumerable.Empty<EasCsvRecord>();

            await _validationErrorLoggerService.LogValidationErrorsAsync(easJobContext, errorModels, cancellationToken);
            await _reportingController.FileLevelErrorReportAsync(easJobContext, easCsvRecords, errorModels, cancellationToken);

            var fileDataCache = _fileDataCacheService.BuildFileDataCache(easJobContext.Ukprn, easJobContext.FileReference, easCsvRecords, null, null, true);
            await _fileDataCacheService.PopulateFileDataCacheAsync(fileDataCache, cancellationToken);
        }

        private ValidationErrorModel BuildValidationError(string ruleName)
        {
            return new ValidationErrorModel
            {
                Severity = SeverityConstants.Error,
                RuleName = ruleName,
                //ErrorMessage = ErrorMessageConstants.FileFormat_01
            };
        }
    }
}
