using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS1920.EF;
using ESFA.DC.EAS1920.EF.Interface;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.EAS.DataService
{
    public class EasSubmissionService : IEasSubmissionService
    {
        private readonly IEasdbContext _easdbContext;
        private readonly ILogger _logger;

        public EasSubmissionService(
            IEasdbContext easdbContext,
            ILogger logger)
        {
            _easdbContext = easdbContext;
            _logger = logger;
        }

        public async Task PersistEasSubmissionAsync(
            List<EasSubmission> easSubmissions,
            List<EasSubmissionValue> easSubmissionValuesList,
            string ukPrn,
            CancellationToken cancellationToken)
        {
            using (var transaction = _easdbContext.Database.BeginTransaction())
            {
                try
                {
                    // Clean up UKPRN data.
                    var previousEasSubmissions = _easdbContext.EasSubmissions.Where(x => x.Ukprn == ukPrn).ToList();
                    foreach (var easSubmission in previousEasSubmissions)
                    {
                        SqlParameter name = new SqlParameter("@SubmissionId", easSubmission.SubmissionId);
                        string commandText = "Delete from Eas_Submission_Values where Submission_Id = @SubmissionId";
                        await _easdbContext.Database.ExecuteSqlCommandAsync(commandText, name);

                        commandText = "Delete from Eas_Submission where Submission_Id = @SubmissionId";
                        await _easdbContext.Database.ExecuteSqlCommandAsync(commandText, name);
                    }

                    // Insert new values
                    foreach (var submission in easSubmissions)
                    {
                        _easdbContext.EasSubmissions.Add(submission);
                    }

                    foreach (var easSubmissionValue in easSubmissionValuesList)
                    {
                        _easdbContext.EasSubmissionValues.Add(easSubmissionValue);
                    }

                    await _easdbContext.SaveChangesAsync(cancellationToken);
                    transaction.Commit();
                    _logger.LogInfo("EAS Submission Persisted Successfully.");
                }
                catch (Exception exception)
                {
                    _logger.LogError("EAS Submission Persist Failed", exception);

                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task PersistEasSubmissionValues(List<EasSubmissionValue> easSubmissionValuesList, CancellationToken cancellationToken)
        {
            await _easdbContext.EasSubmissionValues.AddRangeAsync(easSubmissionValuesList, cancellationToken);
            await _easdbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<EasSubmission>> GetEasSubmissions(Guid submissionId, CancellationToken cancellationToken)
        {
            List<EasSubmission> easSubmissions = await _easdbContext.EasSubmissions.Include(x => x.EasSubmissionValues).Where(x => x.SubmissionId == submissionId).ToListAsync(cancellationToken);
            return easSubmissions;
        }

        public async Task<List<EasSubmissionValue>> GetEasSubmissionValues(Guid submissionId, CancellationToken cancellationToken)
        {
            List<EasSubmissionValue> easSubmissionValues = await _easdbContext.EasSubmissionValues.Where(x => x.SubmissionId == submissionId).ToListAsync(cancellationToken);
            return easSubmissionValues;
        }

        public async Task<List<EasSubmissionValue>> GetEasSubmissionValuesAsync(string UkPrn, CancellationToken cancellationToken)
        {
            List<EasSubmissionValue> easSubmissionValues = new List<EasSubmissionValue>();
            EasSubmission easSubmission = await _easdbContext.EasSubmissions.Where(x => x.Ukprn == UkPrn).OrderByDescending(x => x.UpdatedOn).FirstOrDefaultAsync(cancellationToken);
            if (easSubmission != null)
            {
                easSubmissionValues = easSubmission.EasSubmissionValues.ToList();
            }

            return easSubmissionValues;
        }
    }
}