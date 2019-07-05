using System;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Model;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.EAS.Service.Helpers
{
    public class FileHelper : IFileHelper
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public FileHelper(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public EasFileInfo GetEASFileInfo(IJobContextMessage jobContextMessage)
        {
            // UKPRN/EASDATA-10000421-20180909-121212.csv

            if (!jobContextMessage.KeyValuePairs.ContainsKey(JobContextMessageKey.Filename))
            {
                throw new ArgumentException($"{nameof(JobContextMessageKey.Filename)} is required");
            }

            if (!jobContextMessage.KeyValuePairs.ContainsKey("ReturnPeriod"))
            {
                throw new ArgumentException($"Return Period is required");
            }

            var returnPeriod = Convert.ToInt32(jobContextMessage.KeyValuePairs["ReturnPeriod"].ToString());

            var fileName = jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename].ToString();
            var fileNameParts = fileName.Substring(0, fileName.IndexOf('.')).Split('-');
            if (fileNameParts.Length != 4)
            {
                throw new ArgumentException($"{nameof(JobContextMessageKey.Filename)} is invalid");
            }

            var ukprn = fileNameParts[1];
            var datePart = fileNameParts[2];
            var timePart = fileNameParts[3];

            var filePreparationDate = _dateTimeProvider.ConvertUkToUtc(string.Format($"{datePart}-{timePart}"));

            var fileInfo = new EasFileInfo
            {
                JobId = jobContextMessage.JobId,
                FilePreparationDate = filePreparationDate,
                FileName = fileName,
                DateTime = jobContextMessage.SubmissionDateTimeUtc,
                UKPRN = ukprn,
                ReturnPeriod = returnPeriod
            };

            return fileInfo;
        }
    }
}
