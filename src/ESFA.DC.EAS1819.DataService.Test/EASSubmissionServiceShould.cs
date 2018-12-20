using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace ESFA.DC.EAS1819.DataService.Test
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using ESFA.DC.EAS1819.DataService;
    using ESFA.DC.EAS1819.EF;
    using Xunit;

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
            EasContext easdbContext = new EasContext(options);
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
                    PaymentId = 1,
                    PaymentValue = (decimal)12.22
                },

                new EasSubmissionValue
                {
                    CollectionPeriod = 8,
                    SubmissionId = submissionId,
                    PaymentId = 2,
                    PaymentValue = (decimal)21.22
                },
            };

            EasSubmissionService easSubmissionService = new EasSubmissionService(easdbContext, new SeriLogger(new ApplicationLoggerSettings(), new Logging.ExecutionContext(), null));
            easSubmissionService.PersistEasSubmissionAsync(easSubmissionList, easSubmissionValuesList, "10023139", CancellationToken.None).GetAwaiter().GetResult();

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
            Assert.Equal(1, easSubmissionValueFirst.PaymentId);
            Assert.Equal((decimal)12.22, easSubmissionValueFirst.PaymentValue);
        }
    }
}
