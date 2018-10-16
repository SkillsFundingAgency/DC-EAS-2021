using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.EF.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.EAS1819.DataService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ESFA.DC.EAS1819.DataService.Interface;
    using ESFA.DC.EAS1819.EF;

    public class EasSubmissionService : IEasSubmissionService
    {
        private readonly IRepository<EasSubmission> _easSubmissionRepository;
        private readonly IRepository<EasSubmissionValues> _easSubmissionValuesRepository;
        private readonly IEasdbContext _easdbContext;
        private readonly ILogger _logger;

        public EasSubmissionService(
            IRepository<EasSubmission> easSubmissionRepository,
            IRepository<EasSubmissionValues> easSubmissionValuesRepository,
            IEasdbContext easdbContext,
            ILogger logger)
        {
            _easSubmissionRepository = easSubmissionRepository;
            _easSubmissionValuesRepository = easSubmissionValuesRepository;
            _easdbContext = easdbContext;
            _logger = logger;
        }

        public void PersistEasSubmission(List<EasSubmission> easSubmissions, List<EasSubmissionValues> easSubmissionValuesList)
        {
            using (var transaction = _easdbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var easSubmission in easSubmissions)
                    {
                        _easdbContext.EasSubmission.Add(easSubmission);
                    }

                    foreach (var easSubmissionValue in easSubmissionValuesList)
                    {
                        _easdbContext.EasSubmissionValues.Add(easSubmissionValue);
                    }

                    _easdbContext.SaveChanges();
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

        public void PersistEasSubmissionValues(List<EasSubmissionValues> easSubmissionValuesList)
        {
            foreach (var easSubmissionValue in easSubmissionValuesList)
            {
                _easSubmissionValuesRepository.Insert(easSubmissionValue);
            }
        }

        public List<EasSubmission> GetEasSubmissions(Guid submissionId)
        {
            var easSubmissions = _easSubmissionRepository.AllIncluding(x => x.SubmissionValues).Where(x => x.SubmissionId == submissionId);
            return easSubmissions.ToList();
        }

        public List<EasSubmissionValues> GetEasSubmissionValues(Guid submissionId)
        {
            var easSubmissionValues = _easSubmissionValuesRepository.TableNoTracking.Where(x => x.SubmissionId == submissionId);
            return easSubmissionValues.ToList();
        }
    }
}