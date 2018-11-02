using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Autofac;
using ESFA.DC.EAS1819.DataService.FCS;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Interface.Reports;
using ESFA.DC.EAS1819.Interface.Validation;
using ESFA.DC.EAS1819.Service;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using Xunit;

namespace ESFA.DC.EAS1819.Acceptance.Test
{
    public partial class EasAcceptanceTests
    {
        [Theory]
        //[InlineData("EASDATA-10002143-20181026-140249.csv", "10002143", 248, 496)]
        [InlineData("EASDATA-10000421-20180811-111111.csv", "10000421", 1, 3)]
        public void ProcessEASFile(string filename, string ukPrn, int expectedSubmissionValuesCount, int expectedValidationErrorsCount)
        {
            var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];
            var easdbContext = new EasdbContext(connString);
            List<EasSubmissionValues> easSubmissionValues = new List<EasSubmissionValues>();
            List<ValidationError> validationErrors = new List<ValidationError>();
            //CleanUp(ukPrn, easdbContext);

            var easSubmissions = easdbContext.EasSubmission.ToList();

            var jobContextMessage = BuildJobContextMessage(filename, ukPrn);
            var builder = new ContainerBuilder();
            DIComposition.RegisterTypes(builder);
            var container = builder.Build();

            EntryPoint entryPoint = new EntryPoint(
                new SeriLogger(new ApplicationLoggerSettings(), new Logging.ExecutionContext(), null),
                container.Resolve<IEASDataProviderService>(),
                container.Resolve<IValidationService>(),
                container.Resolve<IReportingController>(),
                container.Resolve<IFileHelper>());

            var tasks = container.Resolve<IList<IEasServiceTask>>();

            var result = entryPoint.CallbackAsync(jobContextMessage, CancellationToken.None, tasks).GetAwaiter().GetResult();

            var easSubmission = easdbContext.EasSubmission.FirstOrDefault(x => x.Ukprn == ukPrn);
            if (easSubmission != null)
            {
                easSubmissionValues = easdbContext.EasSubmissionValues.Where(x => x.SubmissionId == easSubmission.SubmissionId).ToList();
            }

            var sourceFile = easdbContext.SourceFiles.FirstOrDefault(x => x.UKPRN == ukPrn);
            if (sourceFile != null)
            {
                validationErrors = easdbContext.ValidationErrors.Where(x => x.SourceFileId == sourceFile.SourceFileId).ToList();
            }

            Assert.Equal(expectedSubmissionValuesCount, easSubmissionValues.Count);
            Assert.Equal(expectedValidationErrorsCount, validationErrors.Count);
        }

        private static void CleanUp(string ukPrn, EasdbContext easdbContext)
        {
            var easSubmission = easdbContext.EasSubmission.FirstOrDefault(x => x.Ukprn == ukPrn);
            if (easSubmission != null)
            {
                easdbContext.Database.ExecuteSqlCommand(
                    $"Delete from Eas_Submission where Submission_Id = '{easSubmission.SubmissionId}'");
                easdbContext.Database.ExecuteSqlCommand(
                    $"Delete from Eas_Submission_Values where Submission_Id = '{easSubmission.SubmissionId}'");
            }

            var sourceFile = easdbContext.SourceFiles.FirstOrDefault(x => x.UKPRN == ukPrn);
            if (sourceFile != null)
            {
                easdbContext.Database.ExecuteSqlCommand(
                    $"Delete from sourceFile where SourceFileId = {sourceFile.SourceFileId}");
                easdbContext.Database.ExecuteSqlCommand(
                    $"Delete from ValidationError where SourceFileId = {sourceFile.SourceFileId}");
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
                    { "UkPrn", ukPrn }
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
