using System;
using System.Collections.Generic;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.Service.Validation;
using ESFA.DC.ReferenceData.FCS.Model;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace ESFA.DC.EAS1819.Service.Test.Validators.BusinessValidator
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
                CalendarMonth = 8,
                CalendarYear = 2018,
                Value = 1,
                FundingLine = fundingLine,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(null, null, paymentTypes, dateTimeProviderMock.Object);
            _validator.ShouldHaveValidationErrorFor(x => x.FundingLine, easRecord).WithErrorCode("FundingLine_01");
        }

        [Fact]
        public void HaveError_When_ContractTypeIsNotFound_For_A_FundingLine()
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = 8,
                CalendarYear = 2018,
                Value = 1,
                FundingLine = "FundingLineWithoutContract",
                AdjustmentType = "adjustmentType"
            };
            _fundingLineContractMappings = new List<FundingLineContractMapping>()
            {
                new FundingLineContractMapping
                    { FundingLine = "FundingLine", ContractTypeRequired = "APPS1819" }
            };

            _contractAllocations = new List<ContractAllocation>()
            {
                new ContractAllocation
                {
                    FundingStreamPeriodCode = "APPS1819", StartDate = new DateTime(2018, 01, 01),
                    EndDate = new DateTime(2019, 12, 01)
                }
            };

            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractMappings, paymentTypes, dateTimeProviderMock.Object);
            _validator.ShouldHaveValidationErrorFor(x => x.FundingLine, easRecord).WithErrorCode("FundingLine_02");
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
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractMappings, paymentTypes, dateTimeProviderMock.Object);
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
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractMappings, paymentTypes, dateTimeProviderMock.Object);
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
                CalendarMonth = 8,
                CalendarYear = 2018,
                Value = 1,
                FundingLine = fundingLine,
                AdjustmentType = "Adjustment-123+.Type"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2018, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractMappings, paymentTypes, dateTimeProviderMock.Object);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
        }
    }
}