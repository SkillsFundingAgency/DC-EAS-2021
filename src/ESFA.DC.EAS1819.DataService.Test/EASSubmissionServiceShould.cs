using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ExecutionContext = System.Threading.ExecutionContext;

namespace ESFA.DC.EAS1819.Services.Test.Data
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using ESFA.DC.EAS1819.DataService;
    using ESFA.DC.EAS1819.DataService.Interface;
    using ESFA.DC.EAS1819.EF;
    using Xunit;

    public class EasSubmissionServiceShould
    {
        [Fact]
        public void PersistEasSubmissionData()
        {
            var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];
            IRepository<EasSubmission> easSubmissionRepository = new Repository<EasSubmission>(context: new EasdbContext(connString));
            IRepository<EasSubmissionValues> easSubmissionValuesRepository = new Repository<EasSubmissionValues>(context: new EasdbContext(connString));
            var submissionId = Guid.NewGuid();
            var easSubmissionList = new List<EasSubmission>
            {
                new EasSubmission
                {
                    CollectionPeriod = 7,
                    SubmissionId = submissionId,
                    Ukprn = "10023139",
                    ProviderName = "Milton Keynes College",
                    UpdatedOn = DateTime.Now,
                    DeclarationChecked = true,
                    UpdatedBy = "John Smith",
                    NilReturn = false
                },

                new EasSubmission
                {
                    CollectionPeriod = 8,
                    SubmissionId = submissionId,
                    Ukprn = "10023139",
                    ProviderName = "Milton Keynes College",
                    UpdatedOn = DateTime.Now,
                    DeclarationChecked = true,
                    UpdatedBy = "John Smith",
                    NilReturn = false
                }
            };

            var easSubmissionValuesList = new List<EasSubmissionValues>()
            {
                new EasSubmissionValues()
                {
                    CollectionPeriod = 7,
                    SubmissionId = submissionId,
                    PaymentId = 1,
                    PaymentValue = (decimal)12.22
                },

                new EasSubmissionValues()
                {
                    CollectionPeriod = 8,
                    SubmissionId = submissionId,
                    PaymentId = 2,
                    PaymentValue = (decimal)21.22
                },
            };

            EasSubmissionService easSubmissionService = new EasSubmissionService(easSubmissionRepository, easSubmissionValuesRepository, new EasdbContext(connString), new SeriLogger(new ApplicationLoggerSettings(), new Logging.ExecutionContext(), null));
            easSubmissionService.PersistEasSubmission(easSubmissionList, easSubmissionValuesList);

            var easSubmissions = easSubmissionService.GetEasSubmissions(submissionId);
            var submission = easSubmissions.FirstOrDefault();
            Assert.NotNull(easSubmissions);
            Assert.NotNull(submission);
            Assert.NotEmpty(easSubmissions);
            Assert.Equal(submissionId, submission.SubmissionId);
            Assert.Equal(7, submission.CollectionPeriod);
            Assert.Equal("10023139", submission.Ukprn);
            Assert.Equal("Milton Keynes College", submission.ProviderName);
            Assert.True(submission.DeclarationChecked);
            Assert.Equal("John Smith", submission.UpdatedBy);
            Assert.False(submission.NilReturn);

            var easSubmissionValueFirst = submission.SubmissionValues.ElementAt(0);
            Assert.Equal(submissionId, easSubmissionValueFirst.SubmissionId);
            Assert.Equal(7, easSubmissionValueFirst.CollectionPeriod);
            Assert.Equal(1, easSubmissionValueFirst.PaymentId);
            Assert.Equal((decimal)12.22, easSubmissionValueFirst.PaymentValue);
        }
    }
}
