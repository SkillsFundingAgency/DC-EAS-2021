using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.EF;

namespace ESFA.DC.EAS1819.DataService.Interface
{
    public interface IEasSubmissionService
    {
        Task PersistEasSubmissionAsync(List<EasSubmission> easSubmissionsList, List<EasSubmissionValues> easSubmissionValuesList, CancellationToken cancellationToken);

        List<EasSubmission> GetEasSubmissions(Guid submissionId);

        void PersistEasSubmissionValues(List<EasSubmissionValues> easSubmissionValuesList);

        List<EasSubmissionValues> GetEasSubmissionValues(Guid submissionId);
    }
}
