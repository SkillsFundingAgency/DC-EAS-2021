using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.ReportingService.Reports;
using ESFA.DC.EAS.Tests.Base.Builders;
using ESFA.DC.JobContextManager.Model.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.EAS.ReportingService.Test.Reports
{
    public class TestViolationReport
    {
        [Fact]
        public async Task TestViolationReportGeneration()
        {
            var ukprn = "12345678";
            var jobId = 100;
            var reportName = "EAS Rule Violation Report";
            var filename = $"12345678_100_EAS Rule Violation Report-12345678-{new DateTime():yyyyMMdd-HHmmss}.csv";
            var container = "container";

            var jobContextMock = new Mock<IJobContextMessage>();
            var keyValuePairs = new Dictionary<string, object>()
            {
                {"Filename", filename},
                {"UkPrn", ukprn},
                {"Container", container}
            };
            jobContextMock.Setup(jc => jc.KeyValuePairs).Returns(keyValuePairs);

            var fileNameServiceMock = new Mock<IFileNameService>();
            var csvServiceMock = new Mock<ICsvService>();

            fileNameServiceMock.Setup(fns => fns.GetFilename(ukprn, jobId, reportName, It.IsAny<DateTime>(), OutputTypes.Csv)).Returns(filename);

            ViolationReport report = new ViolationReport(fileNameServiceMock.Object, csvServiceMock.Object);

            var result = await report.GenerateReportAsync(jobContextMock.Object, new EasCsvRecordBuilder().Build(), new EasFileInfoBuilder().WithUkPrn(ukprn).WithJobId(jobId).Build(), new ValidationErrorModelBuilder().Build(), CancellationToken.None);

            result.First().Should().Be(filename);
            result.Should().HaveCount(1);
        }
    }
}
