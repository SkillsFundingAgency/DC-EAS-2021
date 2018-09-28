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
        [Theory]
        [InlineData("")]
        [InlineData("InvalidFundingLine")]
        public void HaveError_When_FundingLine_IsNotFound(string fundingLine)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = 8,
                CalendarYear = 2018,
                Value = 1,
                FundingLine = fundingLine,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(dateTimeProviderMock.Object, paymentTypes);
            var result = _validator.Validate(easRecord);
            Assert.False(result.IsValid);
            Assert.Equal("FundingLine_01", result.Errors[0].ErrorCode);
        }

        [Fact]
        public void NotHaveError_When_FundingLine_IsFound()
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = 8,
                CalendarYear = 2018,
                Value = 1,
                FundingLine = paymentTypes[0].FundingLine,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(dateTimeProviderMock.Object, paymentTypes);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("fundingLine")]
        [InlineData("fundingline")]
        [InlineData(" FundingLine ")]
        [InlineData(" Fu@£$%^ndin)(*&^gLine ")]
        [InlineData(" Fundin$$$$$gline ")]
        public void Trims_And_RemovesNonAlphaNumericCharacters_When_Validating_FundingLine(string fundingLine)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = 8,
                CalendarYear = 2018,
                Value = 1,
                FundingLine = fundingLine,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(dateTimeProviderMock.Object, paymentTypes);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(" Funding-123+.Line ")]
        [InlineData(" funding-123+. line ")]
        public void AllowValidAlphaNumericCharacters_When_Validating_FundingLine(string fundingLine)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = 8,
                CalendarYear = 2018,
                Value = 1,
                FundingLine = fundingLine,
                AdjustmentType = "Adjustment-123+.Type"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(dateTimeProviderMock.Object, paymentTypes);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
        }
    }
}