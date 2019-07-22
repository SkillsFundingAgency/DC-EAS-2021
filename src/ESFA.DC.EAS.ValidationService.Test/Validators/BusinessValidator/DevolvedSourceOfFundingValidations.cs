using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ValidationService.Validators;
using ESFA.DC.EAS1920.EF;
using ESFA.DC.ReferenceData.FCS.Model;
using FluentValidation.TestHelper;
using Moq;
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
       
    }
}