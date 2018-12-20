using System;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.ValidationService.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace ESFA.DC.EAS1819.ValidationService.Test.Validators.BusinessValidator
{
    public class AdjustmentTypeValidations : BusinessValidatorBase
    {
        [Theory]
        [InlineData("")]
        [InlineData("InvalidAdjustmentType")]
        public void HaveError_When_AdjustmentType_IsNotFound(string adjustmentType)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2018",
                Value = "1",
                AdjustmentType = adjustmentType
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(null, null, paymentTypes, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.AdjustmentType, easRecord).WithErrorCode("AdjustmentType_01");
        }

        [Fact]
        public void NotHaveError_When_AdjustmentType_IsFound()
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2018",
                Value = "1",
                FundingLine = paymentTypes[0].FundingLine.Name,
                AdjustmentType = paymentTypes[0].AdjustmentType.Name
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
            _validator.ShouldNotHaveValidationErrorFor(x => x.AdjustmentType, easRecord);
        }

        [Theory]
        [InlineData("adjustmenttype")]
        [InlineData(" AdjustmentType ")]
        [InlineData(" Adjustme@£$%^nt)(*&^Type ")]
        [InlineData(" Adjustment$$$$$type ")]
        public void Trims_And_RemovesNonAlphaNumericCharacters_When_Validating_AdjustmentType(string adjustmentType)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2018",
                Value = "1",
                FundingLine = paymentTypes[0].FundingLine.Name,
                AdjustmentType = adjustmentType
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(" Adjustment-123+.Type ")]
        [InlineData(" adjustment  -123+.type ")]
        public void AllowValidAlphaNumericCharacters_When_Validating_AdjustmentType(string adjustmentType)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2018",
                Value = "1",
                FundingLine = "Funding-123+.Line",
                AdjustmentType = adjustmentType
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void HaveError_For_Invalid_FundingLine_Lookup()
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2018",
                Value = "1",
                FundingLine = paymentTypes[0].FundingLine.Name,
                AdjustmentType = "InvalidAdjustmentType"
            };

            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(null, null, paymentTypes, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.AdjustmentType, easRecord).WithErrorCode("AdjustmentType_02");
        }

        [Fact]
        public void NotError_For_valid_FundingLine_Lookup()
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2018",
                Value = "1",
                FundingLine = paymentTypes[0].FundingLine.Name,
                AdjustmentType = paymentTypes[0].AdjustmentType.Name
            };

            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
        }
    }
}