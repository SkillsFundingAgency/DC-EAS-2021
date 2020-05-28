using System;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ValidationService.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace ESFA.DC.EAS.ValidationService.Test.Validators.BusinessValidator
{
    public class FundingLineValidations : BusinessValidatorBase
    {
        [Theory]
        [InlineData("")]
        [InlineData("InvalidFundingLine")]
        public void HaveError_When_FundingLine_IsNotFound(string fundingLine)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2020",
                Value = "1",
                FundingLine = fundingLine,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2019, 09, 01));
            _validator = new BusinessRulesValidator(null, null, paymentTypes, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.FundingLine, easRecord).WithErrorCode("FundingLine_01");
        }

      

        [Fact]
        public void NotHaveError_When_FundingLine_IsFound()
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2020",
                Value = "1",
                FundingLine = paymentTypes[0].FundingLine.Name,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2019, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
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
                CalendarMonth = "8",
                CalendarYear = "2020",
                Value = "1",
                FundingLine = fundingLine,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2019, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(" Funding-123+.Line ")]
        [InlineData(" Funding-123+. line ")]
        public void AllowValidAlphaNumericCharacters_When_Validating_FundingLine(string fundingLine)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2020",
                Value = "1",
                FundingLine = fundingLine,
                AdjustmentType = "Adjustment-123+.Type"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2019, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
        }
    }
}