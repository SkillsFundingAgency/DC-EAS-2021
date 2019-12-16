using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ReportingService.Reports;
using ESFA.DC.EAS.Service.Mapper;
using ESFA.DC.EAS.Tests.Base.Builders;
using ESFA.DC.EAS.Tests.Base.Helpers;
using ESFA.DC.EAS.Tests.Base.Models;
using ESFA.DC.IO.Interfaces;
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
            var csv = string.Empty;
            var filename = $"12345678_100_EAS Funding Report-12345678-{new DateTime():yyyyMMdd-HHmmss}.csv";

            var storage = new Mock<IStreamableKeyValuePersistenceService>();
            var fileNameServiceMock = new Mock<IFileNameService>();
            
            fileNameServiceMock.Setup(fns => fns.GetFilename(reportName, It.IsAny<DateTime>(), OutputTypes.Csv)).Returns(filename);
            fileNameServiceMock.Setup(fns => fns.GetExternalFilename(ukprn, jobId, reportName, It.IsAny<DateTime>(), OutputTypes.Csv)).Returns(filename);
            storage.Setup(x => x.SaveAsync(filename, It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);

            var report = new FundingReport(storage.Object, fileNameServiceMock.Object);           

            await report.GenerateReportAsync(new EasCsvRecordBuilder().GetValidRecords(), new EasFileInfoBuilder().WithUkPrn(ukprn).WithJobId(jobId).Build(), new List<ValidationErrorModel>(), null, CancellationToken.None);
            csv.Should().NotBeNullOrEmpty();
            TestCsvHelper.CheckCsv(csv, new CsvEntry(new EasCsvRecordMapper(), 2));
        }
    }
}
