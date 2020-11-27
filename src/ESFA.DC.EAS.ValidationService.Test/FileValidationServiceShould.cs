using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.Interface.Reports;
using ESFA.DC.EAS.ValidationService.Builders.Interface;
using ESFA.DC.EAS2021.EF;
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
        [InlineData("InvalidHeader.csv", "Fileformat_01")]
        [InlineData("NoHeader.csv", "Fileformat_01")]
        [InlineData("InvalidContent.csv", "Fileformat_02")]
        public async Task ValidateFile_Error(string fileName, string error)
        {
            var cancellationToken = CancellationToken.None;
            var container = "TestFiles";
            var refData = ValidationErrorRefData();

            var easJobContext = new Mock<IEasJobContext>();
            easJobContext.Setup(x => x.Ukprn).Returns(1);
            easJobContext.Setup(x => x.FileReference).Returns(fileName);
            easJobContext.Setup(x => x.Container).Returns(container);

            var validationErrorBuilder = new Mock<IValidationErrorBuilder>();
            var validationErrorLoggerService = new Mock<IValidationErrorLoggerService>();
            var reportingController = new Mock<IReportingController>();
            var fileDataCacheService = new Mock<IFileDataCacheService>();

            var service = NewService(validationErrorBuilder.Object, validationErrorLoggerService.Object, reportingController.Object, fileDataCacheService.Object, FileService());

            var errors = await service.ValidateFile(easJobContext.Object, refData, cancellationToken);

            errors.Should().HaveCount(1);
            errors.First().RuleName.Should().Be(error);

            validationErrorLoggerService.VerifyAll();
            reportingController.VerifyAll();
            fileDataCacheService.VerifyAll();
        }

        [Theory]
        [InlineData("ValidFile.csv")]
        [InlineData("NoContent.csv")]
        public async Task ValidateFile_NoError(string filename)
        {
            var cancellationToken = CancellationToken.None;
            var container = "TestFiles";
            var refData = ValidationErrorRefData();

            var easJobContext = new Mock<IEasJobContext>();

            easJobContext.Setup(x => x.Ukprn).Returns(1);
            easJobContext.Setup(x => x.FileReference).Returns(filename);
            easJobContext.Setup(x => x.Container).Returns(container);

            var validationErrorBuilder = new Mock<IValidationErrorBuilder>();
            var validationErrorLoggerService = new Mock<IValidationErrorLoggerService>();
            var reportingController = new Mock<IReportingController>();
            var fileDataCacheService = new Mock<IFileDataCacheService>();

            var service = NewService(validationErrorBuilder.Object, validationErrorLoggerService.Object, reportingController.Object, fileDataCacheService.Object, FileService());

            var errors = await service.ValidateFile(easJobContext.Object, refData, cancellationToken);

            errors.Should().BeNullOrEmpty();

            validationErrorLoggerService.VerifyAll();
            reportingController.VerifyAll();
            fileDataCacheService.VerifyAll();
        }

        private IFileService FileService()
        {
            return new FileSystemFileService();
        }

        private IReadOnlyDictionary<string, ValidationErrorRule> ValidationErrorRefData()
        {
            return new Dictionary<string, ValidationErrorRule>
            {
                {
                    "Fileformat_01", new ValidationErrorRule()
                },
                {
                    "Fileformat_02", new ValidationErrorRule()
                }
            };
        }

        private FileValidationService NewService(
            IValidationErrorBuilder validationErrorBuilder = null,
            IValidationErrorLoggerService validationErrorLoggerService = null,
            IReportingController reportingController = null,
            IFileDataCacheService fileDataCacheService = null,
            IFileService fileService = null,
            ILogger logger = null)
        {
            return new FileValidationService(validationErrorBuilder, validationErrorLoggerService, reportingController, fileDataCacheService, fileService, logger ?? Mock.Of<ILogger>());
        }
    }
}
