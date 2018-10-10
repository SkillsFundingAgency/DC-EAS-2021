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

        public EasSubmissionService(IRepository<EasSubmission> easSubmissionRepository, IRepository<EasSubmissionValues> easSubmissionValuesRepository)
        {
            _easSubmissionRepository = easSubmissionRepository;
            _easSubmissionValuesRepository = easSubmissionValuesRepository;
        }

        public void PersistEasSubmission(List<EasSubmission> easSubmissions, List<EasSubmissionValues> easSubmissionValuesList)
        {
            foreach (var easSubmission in easSubmissions)
            {
                _easSubmissionRepository.Insert(easSubmission);
            }

            foreach (var easSubmissionValue in easSubmissionValuesList)
            {
                _easSubmissionValuesRepository.Insert(easSubmissionValue);
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