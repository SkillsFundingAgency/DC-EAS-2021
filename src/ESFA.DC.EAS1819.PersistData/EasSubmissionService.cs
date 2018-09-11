using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.EAS1819.Interface;

namespace ESFA.DC.EAS1819.Data
{
    public class EasSubmissionService : IEasSubmissionService
    {
        private readonly IRepository<EasSubmission> _easSubmissionRepository;

        public EasSubmissionService(IRepository<EasSubmission> easSubmissionRepository)
        {
            _easSubmissionRepository = easSubmissionRepository;
        }

        public void PersistEasSubmission(EasSubmission easSubmission)
        {
            _easSubmissionRepository.Insert(easSubmission);
        }

        public List<EasSubmission> GetEasSubmissions(Guid submissionId)
        {
            var easSubmissions = _easSubmissionRepository.AllIncluding(x => x.SubmissionValues).Where(x => x.SubmissionId == submissionId);
            return easSubmissions.ToList();
        }
    }
}