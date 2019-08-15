using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1920.EF;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace ESFA.DC.EAS.DataService.Test
{
    public class EasSubmissionServiceShould
    {
        private readonly ITestOutputHelper _output;

        public EasSubmissionServiceShould(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task PersistEasSubmissionData()
        {
            var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];
            _output.WriteLine(connString);

            DbContextOptions<EasContext> options = new DbContextOptionsBuilder<EasContext>().UseSqlServer(connString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            EasContext easDbContext = new EasContext(options);

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

            var easSubmissionValuesList = new List<EasSubmissionValue>
            {
                new EasSubmissionValue
                {
                    CollectionPeriod = 7,
                    SubmissionId = submissionId,
                    PaymentId = 5,
                    PaymentValue = (decimal)12.22,
                    DevolvedAreaSoF = 110
                },

                new EasSubmissionValue
                {
                    CollectionPeriod = 8,
                    SubmissionId = submissionId,
                    PaymentId = 2,
                    PaymentValue = (decimal)21.22
                }
            };

            EasSubmissionService easSubmissionService = new EasSubmissionService(easDbContext, new SeriLogger(new ApplicationLoggerSettings(), new ExecutionContext(), null));
            await easSubmissionService.PersistEasSubmissionAsync(easSubmissionList, easSubmissionValuesList, "10023139", CancellationToken.None);

            var easSubmissions = await easSubmissionService.GetEasSubmissions(submissionId, CancellationToken.None);
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

            var easSubmissionValueFirst = submission.EasSubmissionValues.ElementAt(0);
            
            Assert.Equal(submissionId, easSubmissionValueFirst.SubmissionId);
            Assert.Equal(7, easSubmissionValueFirst.CollectionPeriod);
            Assert.Equal(5, easSubmissionValueFirst.PaymentId);
            Assert.Equal((decimal)12.22, easSubmissionValueFirst.PaymentValue);
            Assert.Equal(110, easSubmissionValueFirst.DevolvedAreaSoF);

            var secondEasSubmission = easSubmissions[1];
            var easSubmissionValueSecond = secondEasSubmission.EasSubmissionValues.ElementAt(0);
            Assert.Equal(8, easSubmissionValueSecond.CollectionPeriod);
            Assert.Equal(2, easSubmissionValueSecond.PaymentId);
            Assert.Equal((decimal)21.22, easSubmissionValueSecond.PaymentValue);
            //Assert.Equal(null, easSubmissionValueSecond.DevolvedAreaSoF);
        }
    }
}
