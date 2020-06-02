using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.Interface.Validation;
using ESFA.DC.FileService;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.EAS.ValidationService.Test
{
    public class FileValidationServiceShould
    {
        [Theory]
        [InlineData("InvalidHeader.csv")]
        [InlineData("NoHeader.csv")]
        public async Task ValidateFile_Error(string fileName)
        {
            var cancellationToken = CancellationToken.None;
            var container = "TestFiles";
            Stream stream = new MemoryStream();

            var easJobContext = new Mock<IEasJobContext>();

            easJobContext.Setup(x => x.Ukprn).Returns(1);
            easJobContext.Setup(x => x.FileReference).Returns(fileName);
            easJobContext.Setup(x => x.Container).Returns(container);

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            var validationErrorLoggerService = new Mock<IValidationErrorLoggerService>();
            var reportingController = new Mock<IReportingController>();
            var fileDataCacheService = new Mock<IFileDataCacheService>();

            var service = NewService(dateTimeProvider.Object, validationErrorLoggerService.Object, reportingController.Object, fileDataCacheService.Object, FileService());

            var errors = await service.ValidateFile(easJobContext.Object, cancellationToken);

            errors.Should().HaveCount(1);
            errors.First().RuleName.Should().Be("Fileformat_01");

            validationErrorLoggerService.VerifyAll();
            reportingController.VerifyAll();
            fileDataCacheService.VerifyAll();
        }

        [Fact]
        public async Task ValidateFile_NoError()
        {
            var cancellationToken = CancellationToken.None;
            var filename = "ValidHeader.csv";
            var container = "TestFiles";
            Stream stream = new MemoryStream();

            var easJobContext = new Mock<IEasJobContext>();

            easJobContext.Setup(x => x.Ukprn).Returns(1);
            easJobContext.Setup(x => x.FileReference).Returns(filename);
            easJobContext.Setup(x => x.Container).Returns(container);

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            var validationErrorLoggerService = new Mock<IValidationErrorLoggerService>();
            var reportingController = new Mock<IReportingController>();
            var fileDataCacheService = new Mock<IFileDataCacheService>();

            var service = NewService(dateTimeProvider.Object, validationErrorLoggerService.Object, reportingController.Object, fileDataCacheService.Object, FileService());

            var errors = await service.ValidateFile(easJobContext.Object, cancellationToken);

            errors.Should().BeNullOrEmpty();

            validationErrorLoggerService.VerifyAll();
            reportingController.VerifyAll();
            fileDataCacheService.VerifyAll();
        }

        private IFileService FileService()
        {
            return new FileSystemFileService();
        }

        private FileValidationService NewService(
            IDateTimeProvider dateTimeProvider = null,
            IValidationErrorLoggerService validationErrorLoggerService = null,
            IReportingController reportingController = null,
            IFileDataCacheService fileDataCacheService = null,
            IFileService fileService = null,
            ILogger logger = null)
        {
            return new FileValidationService(dateTimeProvider, validationErrorLoggerService, reportingController, fileDataCacheService, fileService, logger ?? Mock.Of<ILogger>());
        }
    }
}
