﻿using System;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ValidationService.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace ESFA.DC.EAS.ValidationService.Test.Validators.BusinessValidator
{
    public class ValueValidations : BusinessValidatorBase
    {
        [Theory]
        [InlineData("$")]
        [InlineData("-")]
        [InlineData("")]
        [InlineData("asfdsafdasfd")]
        public void HaveError_For_Empty_Value(string value)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2018",
                FundingLine = paymentTypes[0].FundingLine.Name,
                AdjustmentType = "adjustmentType",
                Value = value
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(null, null, paymentTypes, null, null, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.Value, easRecord).WithErrorCode("Value_01");
        }

        [Theory]
        [InlineData("-99999999999")]
        [InlineData("9999999999")]
        public void HaveError_For_Invalid_Values(string value)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2018",
                Value = value,
                FundingLine = paymentTypes[0].FundingLine.Name,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(null, null, paymentTypes, null, null, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.Value, easRecord).WithErrorCode("Value_03");
        }
    }
}