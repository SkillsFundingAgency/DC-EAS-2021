using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.EAS1819.Data;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Services.Interface.Data;

namespace ESFA.DC.EAS1819.Services.Data
{
    public class EasSubmissionService : IEasSubmissionService
    {
        private readonly IRepository<EasSubmission> _easSubmissionRepository;
        private readonly IRepository<EasSubmissionValues> _easSubmissionValuesRepository;

        public EasSubmissionService(IRepository<EasSubmission> easSubmissionRepository, IRepository<EasSubmissionValues> easSubmissionValuesRepository)
        {
            _easSubmissionRepository = easSubmissionRepository;
            _easSubmissionValuesRepository = easSubmissionValuesRepository;
        }

        public void PersistEasSubmission(EasSubmission easSubmission)
        {
            _easSubmissionRepository.Insert(easSubmission);
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