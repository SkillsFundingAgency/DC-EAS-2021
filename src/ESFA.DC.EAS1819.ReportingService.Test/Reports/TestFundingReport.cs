using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.ReportingService.Reports;
using ESFA.DC.IO.Interfaces;
using Moq;
using Xunit;

namespace ESFA.DC.EAS1819.ReportingService.Test.Reports
{
    public class TestFundingReport
    {
        Mock<IDateTimeProvider> dateTimeProviderMock;
        Mock<IStreamableKeyValuePersistenceService> storage;

        public TestFundingReport()
        {
            dateTimeProviderMock = new Mock<IDateTimeProvider>();
            storage = new Mock<IStreamableKeyValuePersistenceService>();
        }

        [Fact]
        public async Task TestFundingReportGeneration()
        {
            //FundingReport report = new FundingReport(dateTimeProviderMock.Object, storage.Object);
            //storage.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Callback<string, Stream, CancellationToken>((st, sr, ct) => File.OpenRead("ILR-10033670-1819-20180704-120055-03.xml").CopyTo(sr)).Returns(Task.CompletedTask);
            //storage.Setup(x => x.SaveAsync($"{filename}.csv", It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, string, CancellationToken>((key, value, ct) => csv = value).Returns(Task.CompletedTask);
            //await report.GenerateReportAsync(new List<EasCsvRecord>(), new EasFileInfo(), new List<ValidationErrorModel>(), null, CancellationToken.None);
        }
    }
}
