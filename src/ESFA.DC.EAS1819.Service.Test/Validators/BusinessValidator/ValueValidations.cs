using System;
using System.Collections.Generic;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.Service.Validation;
using Moq;
using Xunit;

namespace ESFA.DC.EAS1819.Service.Test.Validators.BusinessValidator
{
    public partial class BusinessValidatorShould
    {
        [Fact]
        public void HaveError_For_Empty_Value()
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = 8,
                CalendarYear = 2018,
                FundingLine = paymentTypes[0].FundingLine,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(dateTimeProviderMock.Object, paymentTypes);
            var result = _validator.Validate(easRecord);
            Assert.False(result.IsValid);
            Assert.Equal("Value_01", result.Errors[0].ErrorCode);
        }

        [Theory]
        [InlineData(-99999999999)]
        [InlineData(9999999999)]
        public void HaveError_For_Invalid_Values(decimal value)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = 8,
                CalendarYear = 2018,
                Value = value,
                FundingLine = paymentTypes[0].FundingLine,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(dateTimeProviderMock.Object, paymentTypes);
            var result = _validator.Validate(easRecord);
            Assert.False(result.IsValid);
            Assert.Equal("Value_03", result.Errors[0].ErrorCode);
        }
    }
}