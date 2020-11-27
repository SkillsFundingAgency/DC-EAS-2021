using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.BulkCopy.Interfaces;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Config;
using ESFA.DC.EAS.Interface.Constants;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS.DataService.Persist
{
    public class ValidationErrorLoggerService : IValidationErrorLoggerService
    {
        private readonly IEasServiceConfiguration _easServiceConfiguration;
        private readonly IBulkInsert _bulkInsert;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;

        public ValidationErrorLoggerService(
            IEasServiceConfiguration easServiceConfiguration,
            IBulkInsert bulkInsert,
            IDateTimeProvider dateTimeProvider,
            ILogger logger)
        {
            _easServiceConfiguration = easServiceConfiguration;
            _bulkInsert = bulkInsert;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public async Task LogValidationErrorsAsync(IEasJobContext easJobContext, IEnumerable<ValidationErrorModel> validationErrors, CancellationToken cancellationToken)
        {
            var sourceFile = new SourceFile()
            {
                Ukprn = easJobContext.Ukprn.ToString(),
                DateTime = easJobContext.SubmissionDateTimeUtc,
                FileName = easJobContext.FileReference,
                FilePreparationDate = BuildFilePrepDate(easJobContext.FileReference)
            };

            var successfullyCommitted = false;

            _logger.LogInfo("Starting Validation Error persist to DEDs");
            using (var connection = new SqlConnection(_easServiceConfiguration.EasdbConnectionString))
            {
                SqlTransaction transaction = null;
                try
                {
                    await connection.OpenAsync(cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    transaction = connection.BeginTransaction();

                    int sourceFileId = await LogErrorSourceFileAsync(transaction, connection, sourceFile);

                    var validationErrorList = BuildErrors(validationErrors, sourceFileId);

                    await LogValidationErrorsAsync(connection, transaction, validationErrorList, cancellationToken);

                    transaction.Commit();
                    successfullyCommitted = true;

                    _logger.LogInfo("Finished Validation Error persist to DEDs");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed - Validation Error persist to DEDs", ex);
                    throw;
                }
                finally
                {
                    if (!successfullyCommitted)
                    {
                        try
                        {
                            transaction?.Rollback();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Failed to rollback DEDs persist transaction", ex);
                            throw;
                        }
                    }
                }
            }
        }

        public IEnumerable<EasValidationError> BuildErrors(IEnumerable<ValidationErrorModel> validationErrors, int sourceFileId)
        {
            var dateTime = _dateTimeProvider.GetNowUtc();

            return validationErrors?.Select(error => new EasValidationError()
            {
                AdjustmentType = error.AdjustmentType,
                Value = error.Value,
                CalendarMonth = error.CalendarMonth,
                CalendarYear = error.CalendarYear,
                CreatedOn = dateTime,
                ErrorMessage = error.ErrorMessage,
                FundingLine = error.FundingLine,
                RowId = Guid.NewGuid(), //TODO: find out if this is right.
                RuleId = error.RuleName,
                Severity = error.Severity,
                SourceFileId = sourceFileId,
                DevolvedAreaSoF = error.DevolvedAreaSoF,
            }).ToList() ?? Enumerable.Empty<EasValidationError>();
        }

        public DateTime BuildFilePrepDate(string filename)
        {
            var fileNameParts = filename.Substring(0, filename.IndexOf('.')).Split('-');
            if (fileNameParts.Length != 4)
            {
                throw new ArgumentException($"Filename is invalid");
            }

            var ukprn = fileNameParts[1];
            var datePart = fileNameParts[2];
            var timePart = fileNameParts[3];

            return _dateTimeProvider.ConvertUkToUtc(string.Format($"{datePart}-{timePart}"));
        }

        private async Task LogValidationErrorsAsync(SqlConnection sqlConnection, SqlTransaction sqlTransaction, IEnumerable<EasValidationError> validationErrors, CancellationToken cancellationToken)
        {
            await _bulkInsert.Insert(TableNameConstants.ValidationError, validationErrors, sqlConnection, sqlTransaction, cancellationToken);
        }

        private async Task<int> LogErrorSourceFileAsync(SqlTransaction sqlTransaction, SqlConnection sqlConnection, SourceFile sourceFile)
        {
            object output;

            var sql = "INSERT INTO [dbo].[SourceFile] ([FileName], [FilePreparationDate], [UKPRN], [DateTime]) " +
                "OUTPUT INSERTED.SourceFileId " +
                "VALUES (@FileName, @FilePreparationDate, @UKPRN, @DateTime)";

            using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection, sqlTransaction))
            {
                sqlCommand.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = sourceFile.FileName;
                sqlCommand.Parameters.Add("@FilePreparationDate", SqlDbType.DateTime).Value = sourceFile.FilePreparationDate;
                sqlCommand.Parameters.Add("@UKPRN", SqlDbType.NVarChar).Value = sourceFile.Ukprn;
                sqlCommand.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = sourceFile.DateTime;

                sqlCommand.CommandTimeout = 600;

                output = await sqlCommand.ExecuteScalarAsync(CancellationToken.None);
            }

            var id = int.Parse(output.ToString());

            return id;
        }
    }
}
