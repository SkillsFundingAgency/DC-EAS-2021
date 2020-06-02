using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS2021.EF;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.EAS.ValidationService.Test
{
    public class ValidationErrorLoggerServiceTests
    {
        [Fact]
        public async Task LogValidationErrorsAsync()
        {
            var cancellationToken = CancellationToken.None;
            var validationErrors = new List<ValidationErrorModel>();

            var easJobContext = new Mock<IEasJobContext>();

            easJobContext.Setup(x => x.Ukprn).Returns(1);
            easJobContext.Setup(x => x.FileReference).Returns("EASDATA-10000116-20201026-151515.csv");
            easJobContext.Setup(x => x.Container).Returns("Container");
            easJobContext.Setup(x => x.SubmissionDateTimeUtc).Returns(new DateTime(2020, 8, 1));

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            var validationErrorService = new Mock<IValidationErrorService>();

            var service = NewService(validationErrorService.Object, dateTimeProvider.Object);

            await service.LogValidationErrorsAsync(easJobContext.Object, validationErrors, cancellationToken);

            dateTimeProvider.VerifyAll();
            validationErrorService.VerifyAll();
        }

        [Fact]
        public void BuildErrors()
        {
            var validationErrors = new List<ValidationErrorModel>
            {
                new ValidationErrorModel
                {
                    CalendarMonth = "8",
                    CalendarYear = "2020",
                }
            };

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 8, 1));

            var validationErrorsEF = NewService(dateTimeProvider: dateTimeProvider.Object).BuildErrors(validationErrors, 1);

            validationErrorsEF.First().CalendarMonth.Should().Be("8");
            validationErrorsEF.First().CalendarYear.Should().Be("2020");
            validationErrorsEF.First().CreatedOn.Should().Be(new DateTime(2020, 8, 1));
            validationErrorsEF.First().SourceFileId.Should().Be(1);

            dateTimeProvider.VerifyAll();
        }

        [Fact]
        public void BuildErrors_EmptySet()
        {
            var validationErrors = new List<ValidationErrorModel>();
            var validationErrorsEF = new List<ValidationError>();

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 8, 1));

            NewService(dateTimeProvider: dateTimeProvider.Object).BuildErrors(validationErrors, 1).Should().BeEquivalentTo(validationErrorsEF);

            dateTimeProvider.VerifyAll();
        }

        [Fact]
        public void BuildErrors_Null()
        {
            var validationErrors = new List<ValidationErrorModel>();
            var validationErrorsEF = new List<ValidationError>();

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 8, 1));

            NewService(dateTimeProvider: dateTimeProvider.Object).BuildErrors(null, 1).Should().BeEquivalentTo(validationErrorsEF);

            dateTimeProvider.VerifyAll();
        }

        [Fact]
        public void BuildFilePrepDate()
        {
            var filename = "EASDATA-10000116-20201026-151515.csv";
            var expectedDate = new DateTime(2020, 10, 26, 15, 15, 15);

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.ConvertUkToUtc("20201026-151515", "yyyyMMdd-HHmmss")).Returns(expectedDate);

            NewService(dateTimeProvider: dateTimeProvider.Object).BuildFilePrepDate(filename).Should().Be(expectedDate);

            dateTimeProvider.VerifyAll();
        }

        private ValidationErrorLoggerService NewService(IValidationErrorService validationErrorService = null, IDateTimeProvider dateTimeProvider = null)
        {
            return new ValidationErrorLoggerService(validationErrorService, dateTimeProvider);
        }
    }   
}
