using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.EF;

namespace ESFA.DC.EAS1819.DataService.Interface
{
    public interface IEasSubmissionService
    {
        Task PersistEasSubmissionAsync(
            List<EasSubmission> easSubmissionsList,
            List<EasSubmissionValue> easSubmissionValuesList,
            string ukPrn,
            CancellationToken cancellationToken);

        Task<List<EasSubmission>> GetEasSubmissions(Guid submissionId, CancellationToken cancellationToken);

        Task PersistEasSubmissionValues(List<EasSubmissionValue> easSubmissionValuesList, CancellationToken cancellationToken);

        Task<List<EasSubmissionValue>> GetEasSubmissionValues(Guid submissionId, CancellationToken cancellationToken);

        Task<List<EasSubmissionValue>> GetEasSubmissionValuesAsync(string ukPrn, CancellationToken cancellationToken);
    }
}
