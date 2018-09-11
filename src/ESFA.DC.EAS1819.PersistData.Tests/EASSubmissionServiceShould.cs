using ESFA.DC.EAS1819.Data;
using ESFA.DC.EAS1819.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Internal;
using Xunit;

namespace ESFA.DC.EAS1819.PersistData.Tests
{
    public class EasSubmissionServiceShould
    {
        [Fact]
        public void PersistEasSubmissionData()
        {
            var optionsBuilder = new DbContextOptionsBuilder<EasdbContext>();
            var connString = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings.Settings["EasdbConnectionString"].Value;
            optionsBuilder.UseSqlServer(connString);
            IRepository<EasSubmission> easSubmissionRepository = new Repository<EasSubmission>(context: new EasdbContext(optionsBuilder.Options));

            var submissionId = Guid.NewGuid();
            var easSubmission = new EasSubmission
            {
                CollectionPeriod = 7,
                SubmissionId = submissionId,
                Ukprn = "10023139",
                ProviderName = "Milton Keynes College",
                UpdatedOn = DateTime.Now,
                DeclarationChecked = true,
                UpdatedBy = "John Smith",
                NilReturn = false
            };

            var easSubmissionValuesList = new List<EasSubmissionValues>()
            {
                new EasSubmissionValues()
                {
                    CollectionPeriod = easSubmission.CollectionPeriod,
                    SubmissionId = submissionId,
                    PaymentId = 1,
                    PaymentValue = (decimal) 12.22

                },
                new EasSubmissionValues()
                {
                    CollectionPeriod = easSubmission.CollectionPeriod,
                    SubmissionId = submissionId,
                    PaymentId = 2,
                    PaymentValue = (decimal) 21.22

                },

            };
            easSubmission.SubmissionValues = easSubmissionValuesList;

            EasSubmissionService easSubmissionService = new EasSubmissionService(easSubmissionRepository);
            easSubmissionService.PersistEasSubmission(easSubmission);

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

            Assert.Equal(2, submission.SubmissionValues.Count);

            var easSubmissionValueFirst = submission.SubmissionValues.ElementAt(0);
            Assert.Equal(submissionId,easSubmissionValueFirst.SubmissionId);
            Assert.Equal(7,easSubmissionValueFirst.CollectionPeriod);
            Assert.Equal(1,easSubmissionValueFirst.PaymentId);
            Assert.Equal((decimal)12.22,easSubmissionValueFirst.PaymentValue);

        }
    }
}
