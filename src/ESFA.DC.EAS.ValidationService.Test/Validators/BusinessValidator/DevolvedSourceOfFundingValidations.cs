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
        //[Theory]
        //[InlineData("Adult Education - Eligible for MCA/GLA funding (non-procured)")]
        //[InlineData("Adult Education - Eligible for MCA/GLA funding (procured)")]
        //public void NotHaveError_When_ContractTypeIsNotRequiredForFundline(string fundline)
        //{
        //    var easRecord = new EasCsvRecord()
        //    {
        //        CalendarMonth = "8",
        //        CalendarYear = "2019",
        //        Value = "1",
        //        FundingLine = fundline,
        //        AdjustmentType = "adjustmentType"
        //    };
        //    dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2019, 09, 01));
        //    _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
        //    var result = _validator.Validate(easRecord);
        //    Assert.False(result.Errors.Any(x => x.ErrorCode.Equals("FundingLine_02")));
            
        //}

        //[Theory]
        //[InlineData("fundingLine")]
        //[InlineData("fundingline")]
        //[InlineData(" FundingLine ")]
        //[InlineData(" Fu@£$%^ndin)(*&^gLine ")]
        //[InlineData(" Fundin$$$$$gline ")]
        //public void Trims_And_RemovesNonAlphaNumericCharacters_When_Validating_FundingLine(string fundingLine)
        //{
        //    var easRecord = new EasCsvRecord()
        //    {
        //        CalendarMonth = "8",
        //        CalendarYear = "2019",
        //        Value = "1",
        //        FundingLine = fundingLine,
        //        AdjustmentType = "adjustmentType"
        //    };
        //    dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2019, 09, 01));
        //    _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
        //    var result = _validator.Validate(easRecord);
        //    Assert.True(result.IsValid);
        //}

     
    }
}