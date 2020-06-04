using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS.DataService.Interface.FCS;
using ESFA.DC.EAS.DataService.Interface.Postcodes;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.Tests.Base.Builders;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Logging.Interfaces;
using Moq;
using Xunit;

namespace ESFA.DC.EAS.ValidationService.Test
{
    public class ValidationServiceShould
    {
        private readonly Mock<IFundingLineContractTypeMappingDataService> _fundingLineContractTypeMock;
        private readonly Mock<IFCSDataService> _fcsDataServiceMock;
        private readonly Mock<IPostcodesDataService> _postcodesDataServiceMock;
        private readonly EasValidationService _validationService;
        private readonly Mock<IEasPaymentService> _easPaymentServiceMock;
        private readonly Mock<IValidationErrorRetrievalService> _validationErrorServiceMock;
        private readonly Mock<IValidationErrorRuleService> _validationErrorRuleServiceMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly Mock<IEASFileDataProviderService> _fileDataProviderServiceMock;
        private readonly Mock<IFileDataCacheService> _fileDataCacheServiceMock;
        private readonly Mock<ILogger> _loggerMock;

        public ValidationServiceShould()
        {
            _easPaymentServiceMock = new Mock<IEasPaymentService>();
            _validationErrorServiceMock = new Mock<IValidationErrorRetrievalService>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _fcsDataServiceMock = new Mock<IFCSDataService>();
            _postcodesDataServiceMock = new Mock<IPostcodesDataService>();
            _fundingLineContractTypeMock = new Mock<IFundingLineContractTypeMappingDataService>();
            _validationErrorRuleServiceMock = new Mock<IValidationErrorRuleService>();
            _fileServiceMock = new Mock<IFileService>();
            _fileDataProviderServiceMock = new Mock<IEASFileDataProviderService>();
            _fileDataCacheServiceMock = new Mock<IFileDataCacheService>();
            _loggerMock = new Mock<ILogger>();

            _fcsDataServiceMock.Setup(x => x.GetContractsForProviderAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ContractAllocationsBuilder().Build);
            _fcsDataServiceMock.Setup(x => x.GetDevolvedContractsForProviderAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new DevolvedContractsBuilder().Build);
            _postcodesDataServiceMock.Setup(x => x.GetMcaShortCodesForSofCodesAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new McaSofDictionaryBuilder().Build);
            _fundingLineContractTypeMock.Setup(x => x.GetAllFundingLineContractTypeMappings(It.IsAny<CancellationToken>())).ReturnsAsync(new FundingLineContractTypeMappingsBuilder().Build);
            _easPaymentServiceMock.Setup(x => x.GetAllPaymentTypes(It.IsAny<CancellationToken>())).ReturnsAsync(new PaymentTypesBuilder().GetPaymentTypeList);
            _dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(DateTime.UtcNow);
            _fileDataProviderServiceMock.Setup(x => x.ProvideData("Filename", "Container", It.IsAny<CancellationToken>())).ReturnsAsync(new List<EasCsvRecord>
            {
                new EasCsvRecord { CalendarMonth = "8", CalendarYear = "2019", AdjustmentType = "Type", FundingLine = "16-18 Apprenticeships", Value = "1"  }
            });

            _validationErrorRuleServiceMock.Setup(x => x.GetAllValidationErrorRules(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationErrorRuleBuilder().Build());

            _validationService = new EasValidationService(
                _easPaymentServiceMock.Object,
                _dateTimeProviderMock.Object,
                _validationErrorServiceMock.Object,
                _fcsDataServiceMock.Object,
                _postcodesDataServiceMock.Object,
                _fundingLineContractTypeMock.Object,
                _validationErrorRuleServiceMock.Object,
                _fileServiceMock.Object,
                _fileDataProviderServiceMock.Object,
                _fileDataCacheServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public void ValidateEasRecords()
        {
            var easContext = new Mock<IEasJobContext>();

            easContext.Setup(x => x.FileReference).Returns("Filename");
            easContext.Setup(x => x.Ukprn).Returns(1);
            easContext.Setup(x => x.ReturnPeriod).Returns(4);
            easContext.Setup(x => x.Container).Returns("Container");
            easContext.Setup(x => x.SubmissionDateTimeUtc).Returns(DateTime.UtcNow);
            easContext.Setup(x => x.Tasks).Returns(new List<string>() { "Eas" });
            easContext.Setup(x => x.JobId).Returns(0);

            var validAndInvalidRecords = new EasCsvRecordBuilder().GetValidAndInvalidRecords();
            var validationErrorModels = _validationService.ValidateDataAsync(easContext.Object, CancellationToken.None).GetAwaiter().GetResult();
            Assert.NotNull(validationErrorModels);
            Assert.NotEmpty(validationErrorModels);
            Assert.Equal(3, validationErrorModels.Count);
            Assert.Equal(validationErrorModels.Select(x => x.RuleName).ToList(), new List<string> { "CalendarYear_01", "AdjustmentType_01", "AdjustmentType_02" });
        }
    }
}
