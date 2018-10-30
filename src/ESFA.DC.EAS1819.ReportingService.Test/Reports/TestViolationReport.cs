using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.ReportingService.Mapper;
using ESFA.DC.EAS1819.ReportingService.Reports;
using ESFA.DC.EAS1819.Service.Mapper;
using ESFA.DC.EAS1819.Tests.Base.Builders;
using ESFA.DC.EAS1819.Tests.Base.Helpers;
using ESFA.DC.EAS1819.Tests.Base.Models;
using ESFA.DC.IO.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.EAS1819.ReportingService.Test.Reports
{
    public class TestViolationReport
    {

        [Fact]
        public async Task TestViolationReportGeneration()
        {
            Mock<IStreamableKeyValuePersistenceService> storage = new Mock<IStreamableKeyValuePersistenceService>();
            Mock<IDateTimeProvider> dateTimeProviderMock = new Mock<IDateTimeProvider>();
            string csv = string.Empty;
            System.DateTime dateTime = System.DateTime.UtcNow;
            string filename = $"12345678_100_EAS Violation Report-12345678-{dateTime:yyyyMMdd-HHmmss}";

            ViolationReport report = new ViolationReport(dateTimeProviderMock.Object, storage.Object);
            storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(dateTime);
            dateTimeProviderMock.Setup(x => x.ConvertUtcToUk(It.IsAny<System.DateTime>())).Returns(dateTime);

            await report.GenerateReportAsync(new EasCsvRecordBuilder().Build(), new EasFileInfoBuilder().WithUkPrn("12345678").WithJobId(100).Build(), new ValidationErrorModelBuilder().Build(), null, CancellationToken.None);
            csv.Should().NotBeNullOrEmpty();
            TestCsvHelper.CheckCsv(csv, new CsvEntry(new EasCsvViolationRecordMapper(), 2));
        }
    }
}
