using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.DataService.Interface.FCS;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Service;
using ESFA.DC.EAS1819.Tests.Base.Builders;
using ESFA.DC.EAS1819.ValidationService.Builders;
using Moq;
using Xunit;

namespace ESFA.DC.EAS1819.ValidationService.Test
{
    public class ValidationServiceShould
    {
        private readonly Mock<IFundingLineContractTypeMappingDataService> _fundingLineContractTypeMock;
        private readonly Mock<IFCSDataService> _fcsDataServiceMock;
        private readonly EasValidationService _validationService;
        private readonly Mock<IEasPaymentService> _easPaymentServiceMock;
        private readonly Mock<IValidationErrorService> _validationErrorServiceMock;
        private readonly Mock<IValidationErrorRuleService> _validationErrorRuleServiceMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

        public ValidationServiceShould()
        {
            _easPaymentServiceMock = new Mock<IEasPaymentService>();
            _validationErrorServiceMock = new Mock<IValidationErrorService>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _fcsDataServiceMock = new Mock<IFCSDataService>();
            _fundingLineContractTypeMock = new Mock<IFundingLineContractTypeMappingDataService>();
            _validationErrorRuleServiceMock = new Mock<IValidationErrorRuleService>();

            _fcsDataServiceMock.Setup(x => x.GetContractsForProvider(It.IsAny<int>())).Returns(new ContractAllocationsBuilder().Build);
            _fundingLineContractTypeMock.Setup(x => x.GetAllFundingLineContractTypeMappings()).Returns(new FundingLineContractTypeMappingsBuilder().Build);
            _easPaymentServiceMock.Setup(x => x.GetAllPaymentTypes()).Returns(new PaymentTypesBuilder().GetPaymentTypeList);
            _dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(DateTime.UtcNow);

            _validationErrorRuleServiceMock.Setup(x => x.GetAllValidationErrorRules())
                .Returns(new ValidationErrorRuleBuilder().Build());

            _validationService = new EasValidationService(
                _easPaymentServiceMock.Object,
                _dateTimeProviderMock.Object,
                _validationErrorServiceMock.Object,
                _fcsDataServiceMock.Object,
                _fundingLineContractTypeMock.Object,
                _validationErrorRuleServiceMock.Object);
        }

        [Fact]
        public void ValidateEasRecords()
        {
            var fileInfo = new EasFileInfoBuilder().Build();
            var validAndInvalidRecords = new EasCsvRecordBuilder().GetValidAndInvalidRecords();
            var validationErrorModels = _validationService.ValidateDataAsync(fileInfo, validAndInvalidRecords, CancellationToken.None).GetAwaiter().GetResult();
            Assert.NotNull(validationErrorModels);
            Assert.NotEmpty(validationErrorModels);
            Assert.Equal(3, validationErrorModels.Count);
        }
    }
}
