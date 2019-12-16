using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ReportingService.Mapper;
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
    public class TestViolationReport
    {
        [Fact]
        public async Task TestViolationReportGeneration()
        {
            var ukprn = "12345678";
            var jobId = 100;
            var reportName = "EAS Rule Violation Report";
            var csv = string.Empty;
            var filename = $"12345678_100_EAS Rule Violation Report-12345678-{new DateTime():yyyyMMdd-HHmmss}.csv";

            var storage = new Mock<IStreamableKeyValuePersistenceService>();
            var fileNameServiceMock = new Mock<IFileNameService>();

            fileNameServiceMock.Setup(fns => fns.GetFilename(reportName, It.IsAny<DateTime>(), OutputTypes.Csv)).Returns(filename);
            fileNameServiceMock.Setup(fns => fns.GetExternalFilename(ukprn, jobId, reportName, It.IsAny<DateTime>(), OutputTypes.Csv)).Returns(filename);
            storage.Setup(x => x.SaveAsync(filename, It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);

            ViolationReport report = new ViolationReport(storage.Object, fileNameServiceMock.Object);            

            await report.GenerateReportAsync(new EasCsvRecordBuilder().Build(), new EasFileInfoBuilder().WithUkPrn(ukprn).WithJobId(jobId).Build(), new ValidationErrorModelBuilder().Build(), null, CancellationToken.None);
            csv.Should().NotBeNullOrEmpty();
            TestCsvHelper.CheckCsv(csv, new CsvEntry(new EasCsvViolationRecordMapper(), 2));
        }
    }
}
