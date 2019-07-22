using System;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ValidationService.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace ESFA.DC.EAS.ValidationService.Test.Validators.BusinessValidator
{
    public class DevolvedSourceOfFundingValidations : BusinessValidatorBase
    {
        [Theory]
        [InlineData("0")]
        [InlineData("-10")]
        public void HaveError_When_NotAValidLookUp(string devolvedAreaSourceOfFunding)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2019",
                Value = "1",
                FundingLine = "fundline",
                DevolvedAreaSourceOfFunding = devolvedAreaSourceOfFunding,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2019, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.DevolvedAreaSourceOfFunding, easRecord).WithErrorCode("DevolvedAreaSourceOfFunding_03");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("110")]
        [InlineData("111")]
        [InlineData("112")]
        [InlineData("113")]
        [InlineData("114")]
        [InlineData("115")]
        [InlineData("116")]
        public void NotHaveError_When_DevolvedAreaSourceOfFunding_IsValid(string devolvedAreaSourceOfFunding)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2019",
                Value = "1",
                FundingLine = "fundline",
                DevolvedAreaSourceOfFunding = devolvedAreaSourceOfFunding,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2019, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("DevolvedAreaSourceOfFunding_03"));
        }


        [Theory]
        [InlineData("Adult Education - Eligible for MCA/GLA funding (non-procured)")]
        [InlineData("Adult Education - Eligible for MCA/GLA funding (procured)")]
        public void HaveError_When_DevolvedSourceOfFundingDoesNotExist_For_Required_Fundlines(string fundline)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2019",
                Value = "1",
                FundingLine = fundline,
                DevolvedAreaSourceOfFunding = null,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2019, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.DevolvedAreaSourceOfFunding, easRecord).WithErrorCode("DevolvedAreaSourceOfFunding_01");
        }


        [Theory]
        [InlineData("Adult Education - Eligible for MCA/GLA funding (non-procured)", "110")]
        [InlineData("Adult Education - Eligible for MCA/GLA funding (procured)", "116")]
        public void NotHaveError_When_DevolvedSourceOfFundingExist_For_Required_Fundlines(string fundline, string devolvedSourceOfFunding)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2019",
                Value = "1",
                FundingLine = fundline,
                DevolvedAreaSourceOfFunding = devolvedSourceOfFunding,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2019, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("DevolvedAreaSourceOfFunding_01"));
        }
    }
}