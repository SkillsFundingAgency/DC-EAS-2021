using System;
using System.Collections.Generic;
using ESFA.DC.EAS1819.Data;

namespace ESFA.DC.EAS1819.Services.Interface.Data
{
    public interface IEasSubmissionService
    {
        void PersistEasSubmission(EasSubmission EasSubmission);

        List<EasSubmission> GetEasSubmissions(Guid submissionId);

        void PersistEasSubmissionValues(List<EasSubmissionValues> easSubmissionValuesList);

        List<EasSubmissionValues> GetEasSubmissionValues(Guid submissionId);
    }
}
