using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Service.Helpers;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using Xunit;

namespace ESFA.DC.EAS1819.Service.Test.Helpers
{
    public class FileHelperShould
    {
        FileHelper _fileHelper;

        public FileHelperShould()
        {
            _fileHelper = new FileHelper(new DateTimeProvider.DateTimeProvider());
        }

        [Fact]
       public void Generate_FileInfo_From_A_Given_JobContextMessage()
        {
            IJobContextMessage jobContextMessage = new JobContextMessage()
            {
                JobId = 100,
                KeyValuePairs = new Dictionary<string, object>()
                {
                    { JobContextMessageKey.Filename, "12345678/EASDATA-12345678-20180924-140516.csv" }
                },
                SubmissionDateTimeUtc = new DateTime(2018, 10, 20, 13, 05, 16),
                TopicPointer = 1,
                Topics = default(ArraySegment<ITopicItem>)
            };
            var easFileInfo = _fileHelper.GetEASFileInfo(jobContextMessage);
            Assert.Equal(100, easFileInfo.JobId);
            Assert.Equal("12345678", easFileInfo.UKPRN);
            Assert.Equal(jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename].ToString(), easFileInfo.FileName);
            Assert.Equal(jobContextMessage.SubmissionDateTimeUtc, easFileInfo.DateTime);
            Assert.Equal(new DateTime(2018, 09, 24, 13, 05, 16), easFileInfo.FilePreparationDate);
        }
    }
}
