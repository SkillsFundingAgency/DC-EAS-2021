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
using ESFA.DC.EAS2021.EF;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using Microsoft.EntityFrameworkCore;
using Moq;
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
        [InlineData("EASDATA-10000116-20201026-000001.csv", 10000116, 31, 93)]
        [InlineData("EASDATA-10004375-20201126-121212.csv", 10004375, 0, 4)] // Invalid Calendar year , Calendar Month
        [InlineData("EASDATA-10004376-20200826-000001.csv", 10004376, 1, 4)]// Invalid Value field
        [InlineData("EASDATA-10004376-20200915-040404.csv", 10004376, 0, 1)] // Invalid Header - wrong column name
        [InlineData("EASDATA-10004376-20200915-040405.csv", 10004376, 0, 1)] // Invalid Header - too many columns
        [InlineData("EASDATA-10000116-20201026-000000.csv", 10000116, 0, 0)] // Empty file.
        [InlineData("EASDATA-10004376-20200826-000002.csv", 10004376, 2, 10)]// Invalid Calendar Year and Calendar Month
        [InlineData("EASDATA-10004376-20200826-000003.csv", 10004376, 2, 2)]// Funding line with spaces, testing Cross record and inserting into database.
        [InlineData("EASDATA-10000116-20200131-151800.csv", 10000116, 0, 1)]// Invalid calendar month/year
        [InlineData("EASDATA-10000116-20200131-151801.csv", 10000116, 0, 1)]// Invalid FileFormat_02
        [InlineData("EASDATA-10000116-20201026-151515.csv", 10000116, 3, 1)]// Valid DevolvedSourceOfFunding no contract
        [InlineData("EASDATA-10000116-20201026-161616.csv", 10000116, 0, 6)]// InValid DevolvedSourceOfFunding and Fundingline combination. no contract
        [InlineData("EASDATA-10036143-20200813-153540.csv", 10036143, 2, 14)]// duplicate records with DevolvedSourceOfFunding
        [InlineData("EASDATA-10003909-20200816-100004.csv", 10003909, 62, 1508)]// all combinations

        public async Task ProcessEASFile(string filename, int ukPrn, int expectedSubmissionValuesCount, int expectedValidationErrorsCount)
        {
            var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];
            DbContextOptions<EasContext> options = new DbContextOptionsBuilder<EasContext>().UseSqlServer(connString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            EasContext easdbContext = new EasContext(options);
            List<EasSubmissionValue> easSubmissionValues = new List<EasSubmissionValue>();
            List<ValidationError> validationErrors = new List<ValidationError>();
            var easContextMessage = BuildEasJobContextMessage(filename, ukPrn, 0);
            var builder = new ContainerBuilder();
            _output.WriteLine(connString);
            CleanUp(ukPrn.ToString(), easdbContext);
            DIComposition.RegisterTypes(builder);
            var container = builder.Build();

            EntryPoint entryPoint = new EntryPoint(
                new SeriLogger(new ApplicationLoggerSettings(), new Logging.ExecutionContext(), null),
                container.Resolve<IValidationService>(),
                container.Resolve<IReportingController>());


            var tasks = container.Resolve<IList<IEasServiceTask>>();

            var result = await entryPoint.CallbackAsync(easContextMessage, CancellationToken.None, tasks);

            var easSubmission = easdbContext.EasSubmissions.FirstOrDefault(x => x.Ukprn == ukPrn.ToString());
            if (easSubmission != null)
            {
                easSubmissionValues = easdbContext.EasSubmissionValues.Where(x => x.SubmissionId == easSubmission.SubmissionId).ToList();
            }

            var sourceFile = easdbContext.SourceFiles.FirstOrDefault(x => x.Ukprn == ukPrn.ToString());
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

        private IEasJobContext BuildEasJobContextMessage(string filename, int ukPrn, int jobId)
        {
            var easContext = new Mock<IEasJobContext>();

            easContext.Setup(x => x.FileReference).Returns($"{ukPrn}\\{filename}");
            easContext.Setup(x => x.Ukprn).Returns(ukPrn);
            easContext.Setup(x => x.ReturnPeriod).Returns(4);
            easContext.Setup(x => x.Container).Returns("Container");
            easContext.Setup(x => x.SubmissionDateTimeUtc).Returns(DateTime.UtcNow);
            easContext.Setup(x => x.Tasks).Returns(new List<string>() { "Eas" });
            easContext.Setup(x => x.JobId).Returns(0);

            return easContext.Object;
        }
    }
}
