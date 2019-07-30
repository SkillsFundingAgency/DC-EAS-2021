using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Interface.Validation;
using ESFA.DC.EAS.Service;
using ESFA.DC.EAS1920.EF;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace ESFA.DC.EAS.Acceptance.Test
{
    public partial class EasAcceptanceTests
    {
        private readonly ITestOutputHelper _output;

        public EasAcceptanceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("EASDATA-10000116-20191026-000001.csv", "10000116", 31, 76)]
        [InlineData("EASDATA-10004375-20181126-121212.csv", "10004375", 0, 4)] // Invalid Calendar year , Calendar Month
        [InlineData("EASDATA-10004376-20180826-000001.csv", "10004376", 1, 4)]// Invalid Value field
        [InlineData("EASDATA-10004376-20180915-040404.csv", "10004376", 0, 1)] // Invalid Header
        [InlineData("EASDATA-10000116-20181026-000000.csv", "10000116", 0, 0)] // Empty file.
        [InlineData("EASDATA-10004376-20180826-000002.csv", "10004376", 2, 10)]// Invalid Calendar Year and Calendar Month
        [InlineData("EASDATA-10004376-20180826-000003.csv", "10004376", 2, 2)]// Funding line with spaces, testing Cross record and inserting into database.
        [InlineData("EASDATA-10000116-20190131-151800.csv", "10000116", 0, 1)]// Invalid calendar month/year
        [InlineData("EASDATA-10000116-20191026-151515.csv", "10000116", 3, 0)]// Valid DevolvedSourceOfFunding
        [InlineData("EASDATA-10000116-20191026-161616.csv", "10000116", 0, 5)]// InValid DevolvedSourceOfFunding and Fundingline combination

        public async Task ProcessEASFile(string filename, string ukPrn, int expectedSubmissionValuesCount, int expectedValidationErrorsCount)
        {
            var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];
            DbContextOptions<EasContext> options = new DbContextOptionsBuilder<EasContext>().UseSqlServer(connString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            EasContext easdbContext = new EasContext(options);
            List<EasSubmissionValue> easSubmissionValues = new List<EasSubmissionValue>();
            List<ValidationError> validationErrors = new List<ValidationError>();
            var jobContextMessage = BuildJobContextMessage(filename, ukPrn);
            var builder = new ContainerBuilder();
            _output.WriteLine(connString);
            CleanUp(ukPrn, easdbContext);
            DIComposition.RegisterTypes(builder);
            var container = builder.Build();

            EntryPoint entryPoint = new EntryPoint(
                new SeriLogger(new ApplicationLoggerSettings(), new Logging.ExecutionContext(), null),
                container.Resolve<IEASDataProviderService>(),
                container.Resolve<IValidationService>(),
                container.Resolve<IReportingController>(),
                container.Resolve<IFileHelper>());

            var tasks = container.Resolve<IList<IEasServiceTask>>();

            var result = await entryPoint.CallbackAsync(jobContextMessage, CancellationToken.None, tasks);

            var easSubmission = easdbContext.EasSubmissions.FirstOrDefault(x => x.Ukprn == ukPrn);
            if (easSubmission != null)
            {
                easSubmissionValues = easdbContext.EasSubmissionValues.Where(x => x.SubmissionId == easSubmission.SubmissionId).ToList();
            }

            var sourceFile = easdbContext.SourceFiles.FirstOrDefault(x => x.Ukprn == ukPrn);
            if (sourceFile != null)
            {
                validationErrors = easdbContext.ValidationErrors.Where(x => x.SourceFileId == sourceFile.SourceFileId).ToList();
            }

            Assert.Equal(expectedSubmissionValuesCount, easSubmissionValues.Count);
            Assert.Equal(expectedValidationErrorsCount, validationErrors.Count);
        }

        private static void CleanUp(string ukPrn, EasContext easdbContext)
        {
            var previousEasSubmissions = easdbContext.EasSubmissions.Where(x => x.Ukprn == ukPrn).ToList();
            foreach (var easSubmission in previousEasSubmissions)
            {
                SqlParameter id = new SqlParameter("@SubmissionId", easSubmission.SubmissionId);
                easdbContext.Database.ExecuteSqlCommand(
                    "Delete from Eas_Submission_Values where Submission_Id = @SubmissionId", id);
                easdbContext.Database.ExecuteSqlCommand(
                    "Delete from Eas_Submission where Submission_Id = @SubmissionId", id);
            }

            var previousSourceFiles = easdbContext.SourceFiles.Where(x => x.Ukprn == ukPrn).ToList();
            foreach (var sourceFile in previousSourceFiles)
            {
                SqlParameter id = new SqlParameter("@SubmissionId", sourceFile.SourceFileId);
                easdbContext.Database.ExecuteSqlCommand(
                    "Delete from ValidationError where SourceFileId = @SubmissionId", id);
                easdbContext.Database.ExecuteSqlCommand(
                    "Delete from sourceFile where SourceFileId = @SubmissionId", id);
            }
        }

        private IJobContextMessage BuildJobContextMessage(string filename, string ukPrn)
        {
            IJobContextMessage jobContextMessage = new JobContextMessage()
            {
                JobId = 101,
                KeyValuePairs = new Dictionary<string, object>()
                {
                    { "Filename", filename },
                    { "UkPrn", ukPrn },
                    { "ReturnPeriod", 4 }
                },
                SubmissionDateTimeUtc = DateTime.UtcNow,
                TopicPointer = 0,
                Topics = new List<ITopicItem>()
                {
                    new TopicItem()
                    {
                        SubscriptionName = "Process",
                        Tasks =
                            new List<ITaskItem>()
                            {
                                new TaskItem()
                                {
                                    Tasks = new List<string>() { "Eas" }
                                }
                            }
                    }
                }
            };
            return jobContextMessage;
        }
    }
}
