using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.DataService.Interface.FCS;
using ESFA.DC.EAS.DataService.Interface.Postcodes;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.Tests.Base.Builders;
using ESFA.DC.EAS.ValidationService.Builders.Interface;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.Logging.Interfaces;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace ESFA.DC.EAS.ValidationService.Test
{
    public class ValidationServiceShould
    {
        [Fact]
        public async Task ValidateEasRecords()
        {
            var refData = ValidationErrorRefData();

            var easContext = new Mock<IEasJobContext>();

            easContext.Setup(x => x.FileReference).Returns("Filename");
            easContext.Setup(x => x.Ukprn).Returns(1);
            easContext.Setup(x => x.ReturnPeriod).Returns(4);
            easContext.Setup(x => x.Container).Returns("Container");
            easContext.Setup(x => x.SubmissionDateTimeUtc).Returns(DateTime.UtcNow);
            easContext.Setup(x => x.Tasks).Returns(new List<string>() { "Eas" });
            easContext.Setup(x => x.JobId).Returns(0);

            var easPaymentServiceMock = new Mock<IEasPaymentService>();
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var postcodesDataServiceMock = new Mock<IPostcodesDataService>();
            var fundingLineContractTypeMock = new Mock<IFundingLineContractTypeMappingDataService>();
            var validationErrorBuilderMock = new Mock<IValidationErrorBuilder>();
            var fileDataProviderServiceMock = new Mock<IEASFileDataProviderService>();
            var fileDataCacheServiceMock = new Mock<IFileDataCacheService>();
            var loggerMock = new Mock<ILogger>();

            fcsDataServiceMock.Setup(x => x.GetContractsForProviderAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ContractAllocationsBuilder().Build);
            fcsDataServiceMock.Setup(x => x.GetDevolvedContractsForProviderAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new DevolvedContractsBuilder().Build);
            postcodesDataServiceMock.Setup(x => x.GetMcaShortCodesForSofCodesAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new McaSofDictionaryBuilder().Build);
            fundingLineContractTypeMock.Setup(x => x.GetAllFundingLineContractTypeMappings(It.IsAny<CancellationToken>())).ReturnsAsync(new FundingLineContractTypeMappingsBuilder().Build);
            easPaymentServiceMock.Setup(x => x.GetAllPaymentTypes(It.IsAny<CancellationToken>())).ReturnsAsync(new PaymentTypesBuilder().GetPaymentTypeList);
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(DateTime.UtcNow);
            fileDataProviderServiceMock.Setup(x => x.ProvideData("Filename", "Container", It.IsAny<CancellationToken>())).ReturnsAsync(new List<EasCsvRecord>
            {
                new EasCsvRecord { CalendarMonth = "8", CalendarYear = "2019", AdjustmentType = "Type", FundingLine = "16-18 Apprenticeships", Value = "1"  }
            });

            var expectedErrorModels = new List<ValidationErrorModel>
            {
                new ValidationErrorModel
                {
                    RuleName = "CalendarYear_01",
                    Severity = "E"
                },
                new ValidationErrorModel
                {
                    RuleName = "AdjustmentType_01",
                    Severity = "E"
                },
                new ValidationErrorModel
                {
                    RuleName = "AdjustmentType_02",
                    Severity = "E"
                }
            };

            validationErrorBuilderMock.Setup(x => x.BuildValidationErrors(It.IsAny<List<ValidationResult>>(), refData)).Returns(expectedErrorModels);


            var errorModels = await new EasValidationService(
                easPaymentServiceMock.Object,
                dateTimeProviderMock.Object,
                fcsDataServiceMock.Object,
                postcodesDataServiceMock.Object,
                fundingLineContractTypeMock.Object,
                validationErrorBuilderMock.Object,
                fileDataProviderServiceMock.Object,
                fileDataCacheServiceMock.Object,
                loggerMock.Object).ValidateDataAsync(easContext.Object, refData, CancellationToken.None);

            
            Assert.NotNull(errorModels);
            Assert.NotEmpty(errorModels);
            Assert.Equal(3, errorModels.Count);
            Assert.Equal(errorModels, expectedErrorModels);
        }

        private IReadOnlyDictionary<string, ValidationErrorRule> ValidationErrorRefData()
        {
            return new Dictionary<string, ValidationErrorRule>
            {
                {
                    "CalendarYear_01", new ValidationErrorRule { RuleId = "CalendarYear_01", Severity = "E" }
                },
                {
                    "AdjustmentType_01", new ValidationErrorRule { RuleId = "AdjustmentType_01", Severity = "E" }
                },
                {
                    "AdjustmentType_02", new ValidationErrorRule { RuleId = "AdjustmentType_02", Severity = "E" }
                }
            };
        }
    }
}
