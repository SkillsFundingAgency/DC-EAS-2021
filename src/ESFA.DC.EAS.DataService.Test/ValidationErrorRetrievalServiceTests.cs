using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using ESFA.DC.EAS2021.EF.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.EAS2021.EF;
using MockQueryable.Moq;

namespace ESFA.DC.EAS.DataService.Test
{
    public class ValidationErrorRetrievalServiceTests
    {
        [Fact]
        public async Task GetContractsForProviderAsync()
        {
            var cancellationToken = CancellationToken.None;
            var ukprn = 1;

            var sourceFiles = new List<SourceFile>()
            {
                new SourceFile
                {
                    SourceFileId = 1,
                    Ukprn = "1",
                    FilePreparationDate = new DateTime(2020, 8, 1)
                },
                new SourceFile
                {
                    SourceFileId = 2,
                    Ukprn = "1",
                    FilePreparationDate = new DateTime(2020, 9, 1)
                },
                new SourceFile
                {
                    SourceFileId = 3,
                    Ukprn = "2",
                    FilePreparationDate = new DateTime(2020, 10, 1)
                }
            };

            var validationErrors = new List<ValidationError>()
            {
                new ValidationError
                {
                    SourceFileId = 1,
                    RuleId = "Rule1"
                },
                new ValidationError
                {
                    SourceFileId = 1,
                    RuleId = "Rule2"
                },
                new ValidationError
                {
                    SourceFileId = 2,
                    RuleId = "Rule1"
                },
                new ValidationError
                {
                    SourceFileId = 2,
                    RuleId = "Rule2"
                },
                new ValidationError
                {
                    SourceFileId = 3,
                    RuleId = "Rule1"
                },

            };

            var contextMock = new Mock<IEasdbContext>();
            contextMock.Setup(x => x.SourceFiles).Returns(sourceFiles.AsQueryable().BuildMockDbSet().Object);
            contextMock.Setup(x => x.ValidationErrors).Returns(validationErrors.AsQueryable().BuildMockDbSet().Object);

            var expectedValidationErrors = new List<ValidationError>()
            {
                new ValidationError
                {
                    SourceFileId = 2,
                    RuleId = "Rule1"
                },
                new ValidationError
                {
                    SourceFileId = 2,
                    RuleId = "Rule2"
                },
            };

            var serviceResult = await NewService(contextMock.Object).GetValidationErrorsAsync(ukprn, cancellationToken);

            serviceResult.Should().BeEquivalentTo(expectedValidationErrors);
        }

        private ValidationErrorRetrievalService NewService(IEasdbContext easdbContext)
        {
            return new ValidationErrorRetrievalService(easdbContext, Mock.Of<ILogger>());
        }
    }
}
