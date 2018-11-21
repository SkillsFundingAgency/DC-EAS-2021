using System;
using System.Collections.Generic;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.ValidationService.Validators;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace ESFA.DC.EAS1819.ValidationService.Test.Validators.BusinessValidator
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
            _validator = new BusinessRulesValidator(null, null, paymentTypes, dateTimeProviderMock.Object);
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
            _validator = new BusinessRulesValidator(null, null, paymentTypes, dateTimeProviderMock.Object);
            _validator.ShouldHaveValidationErrorFor(x => x.Value, easRecord).WithErrorCode("Value_03");
        }
    }
}