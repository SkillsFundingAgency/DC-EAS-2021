using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.ReportingService.Reports;
using ESFA.DC.EAS.Tests.Base.Builders;
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
            var ukprn = 12345678;
            var jobId = 100;
            var reportName = "EAS Rule Violation Report";
            var filename = $"12345678_100_EAS Rule Violation Report-12345678-20190801-090000.csv";
            var container = "container";
            var submissionDate = new DateTime(2019, 8, 1, 9, 0, 0);

            var easJobContext = new Mock<IEasJobContext>();

            easJobContext.Setup(jc => jc.FileReference).Returns("Filename");
            easJobContext.Setup(jc => jc.Ukprn).Returns(ukprn);
            easJobContext.Setup(jc => jc.Container).Returns(container);
            easJobContext.Setup(jc => jc.JobId).Returns(jobId);
            easJobContext.Setup(jc => jc.SubmissionDateTimeUtc).Returns(submissionDate);

            var fileNameServiceMock = new Mock<IFileNameService>();
            var csvServiceMock = new Mock<ICsvFileService>();

            fileNameServiceMock.Setup(fns => fns.GetFilename(ukprn.ToString(), jobId, reportName, It.IsAny<DateTime>(), OutputTypes.Csv)).Returns(filename);

            ViolationReport report = new ViolationReport(fileNameServiceMock.Object, csvServiceMock.Object);

            var result = await report.GenerateReportAsync(easJobContext.Object, new EasCsvRecordBuilder().Build(), new ValidationErrorModelBuilder().Build(), CancellationToken.None);

            result.First().Should().Be(filename);
            result.Should().HaveCount(1);

            fileNameServiceMock.VerifyAll();
            csvServiceMock.VerifyAll();
        }
    }
}
