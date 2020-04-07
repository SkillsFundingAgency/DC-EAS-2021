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
using ESFA.DC.JobContextManager.Model.Interface;
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
            var ukprn = "12345678";
            var jobId = 100;
            var reportName = "EAS Funding Report";
            var filename = $"12345678_100_EAS Funding Report-12345678-{new DateTime():yyyyMMdd-HHmmss}.csv";
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
            var csvServiceMock = new Mock<ICsvFileService>();            

            fileNameServiceMock.Setup(fns => fns.GetFilename(ukprn, jobId, reportName, It.IsAny<DateTime>(), OutputTypes.Csv)).Returns(filename);

            var report = new FundingReport(fileNameServiceMock.Object, csvServiceMock.Object);

            var result = await report.GenerateReportAsync(jobContextMock.Object, new EasCsvRecordBuilder().GetValidRecords(), new EasFileInfoBuilder().WithUkPrn(ukprn).WithJobId(jobId).Build(), new List<ValidationErrorModel>(), CancellationToken.None);

            result.First().Should().Be(filename);
            result.Should().HaveCount(1);
        }
    }
}
