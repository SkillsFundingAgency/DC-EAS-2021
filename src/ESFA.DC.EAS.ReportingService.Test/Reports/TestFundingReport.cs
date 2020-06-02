using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ReportingService.Reports;
using ESFA.DC.EAS.Tests.Base.Builders;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.EAS.ReportingService.Test.Reports
{
    public class TestFundingReport
    {
        [Fact]
        public async Task TestFundingReportGeneration()
        {
            var ukprn = 12345678;
            var jobId = 100;
            var reportName = "EAS Funding Report";
            var filename = $"12345678_100_EAS Funding Report-12345678-20200801-090000.csv";
            var container = "container";
            var submissionDate = new DateTime(2020, 8, 1, 9, 0, 0);

            var jobContextMock = new Mock<IEasJobContext>();

            jobContextMock.Setup(jc => jc.FileReference).Returns("Filename");
            jobContextMock.Setup(jc => jc.Ukprn).Returns(ukprn);
            jobContextMock.Setup(jc => jc.Container).Returns(container);
            jobContextMock.Setup(jc => jc.JobId).Returns(jobId);
            jobContextMock.Setup(jc => jc.SubmissionDateTimeUtc).Returns(submissionDate);

            var fileNameServiceMock = new Mock<IFileNameService>();
            var csvServiceMock = new Mock<ICsvFileService>();            

            fileNameServiceMock.Setup(fns => fns.GetFilename(ukprn.ToString(), jobId, reportName, It.IsAny<DateTime>(), OutputTypes.Csv)).Returns(filename);

            var report = new FundingReport(fileNameServiceMock.Object, csvServiceMock.Object);

            var result = await report.GenerateReportAsync(jobContextMock.Object, new EasCsvRecordBuilder().GetValidRecords(), new List<ValidationErrorModel>(), CancellationToken.None);

            result.First().Should().Be(filename);
            result.Should().HaveCount(1);

            fileNameServiceMock.VerifyAll();
            csvServiceMock.VerifyAll();
        }
    }
}
