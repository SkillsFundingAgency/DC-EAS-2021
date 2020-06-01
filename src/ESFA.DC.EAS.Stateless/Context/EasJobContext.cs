using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.EAS.Interface;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model;

namespace ESFA.DC.EAS.Stateless.Context
{
    public class EasJobContext : IEasJobContext
    {
        private readonly JobContextMessage _jobContextMessage;

        public EasJobContext(JobContextMessage jobContextMessage)
        {
            _jobContextMessage = jobContextMessage;
        }

        public string FileReference
        {
            get => _jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename].ToString();
            set => _jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename] = value;
        }

        public string Container => _jobContextMessage.KeyValuePairs[JobContextMessageKey.Container].ToString();

        public IEnumerable<string> Tasks => _jobContextMessage.Topics[_jobContextMessage.TopicPointer].Tasks.SelectMany(x => x.Tasks);

        public int Ukprn
        {
            get => int.Parse(_jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString());
            set => throw new System.NotImplementedException();
        }

        public int ReturnPeriod
        {
            get => int.Parse(_jobContextMessage.KeyValuePairs[JobContextMessageKey.ReturnPeriod].ToString());
            set => _jobContextMessage.KeyValuePairs[JobContextMessageKey.ReturnPeriod] = value;
        }

        public DateTime SubmissionDateTimeUtc => _jobContextMessage.SubmissionDateTimeUtc;

        public long JobId => _jobContextMessage.JobId;

        public string ReportOutputFileNames
        {
            get => _jobContextMessage.KeyValuePairs.ContainsKey("ReportOutputFileNames")
                ? _jobContextMessage.KeyValuePairs["ReportOutputFileNames"].ToString()
                : string.Empty;
            set => _jobContextMessage.KeyValuePairs["ReportOutputFileNames"] = value;
        }
    }
}
