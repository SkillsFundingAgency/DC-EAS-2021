using System;
using System.Collections.Generic;
using ESFA.DC.EAS1819.Data;

namespace ESFA.DC.EAS1819.Interface
{
    public interface IEasSubmissionService
    {
        void PersistEasSubmission(EasSubmission EasSubmission);

        List<EasSubmission> GetEasSubmissions(Guid submissionId);
    }
}
