using System;
using System.Collections.Generic;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ValidationService.Validators;
using ESFA.DC.ReferenceData.FCS.Model;
using FluentValidation.TestHelper;
using Xunit;

namespace ESFA.DC.EAS.ValidationService.Test.Validators.BusinessValidator
{
    public class FundingLineValidations : BusinessValidatorBase
    {
        [Theory]
        [InlineData("")]
        [InlineData("InvalidFundingLine")]
        public void FundingLine_01_HaveError_When_FundingLine_IsNotFound(string fundingLine)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2020",
                Value = "1",
                FundingLine = fundingLine,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 09, 01));
            _validator = new BusinessRulesValidator(null, null, paymentTypes, null, null, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.FundingLine, easRecord).WithErrorCode("FundingLine_01");
        }

      

        [Fact]
        public void FundingLine_01_NotHaveError_When_FundingLine_IsFound()
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2020",
                Value = "1",
                FundingLine = paymentTypes[0].FundingLine.Name,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, null, null, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("fundingLine")]
        [InlineData("fundingline")]
        [InlineData(" FundingLine ")]
        [InlineData(" Fu@£$%^ndin)(*&^gLine ")]
        [InlineData(" Fundin$$$$$gline ")]
        public void FundingLine_01_Trims_And_RemovesNonAlphaNumericCharacters_When_Validating_FundingLine(string fundingLine)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2020",
                Value = "1",
                FundingLine = fundingLine,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, null, null, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(" Funding-123+.Line ")]
        [InlineData(" Funding-123+. line ")]
        public void FundingLine_01_AllowValidAlphaNumericCharacters_When_Validating_FundingLine(string fundingLine)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2020",
                Value = "1",
                FundingLine = fundingLine,
                AdjustmentType = "Adjustment-123+.Type"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, null, null, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
        }

        [Theory(Skip = "bug 98829 : contract re-issue with ended dates that the business want to be valid for now")]
        [InlineData("2021", "08")]
        [InlineData("2022", "01")]
        public void FundingLine_02_Have_Error_WhenContractEndDate_IsLessThan_EasRecordDate(string year, string month)
        {
            _contractAllocations = new List<ContractAllocation>()
            {
                new ContractAllocation()
                {
                    FundingStreamPeriodCode = "APPS2021",
                    EndDate = new DateTime(2021, 07, 31)
                }
            };

            var easRecord = new EasCsvRecord()
            {
                AdjustmentType = "adjustment",
                FundingLine = "FundingLine2021",
                CalendarYear = year,
                CalendarMonth = month,
                Value = "10"
            };
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, null, null, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.FundingLine, easRecord).WithErrorCode("FundingLine_02");
        }

        [Theory]
        [InlineData("2021", "07")]
        [InlineData("2021", "06")]
        public void FundingLine_02_No_Error_WhenContractEndDate_IsGreaterThanOrEqualTo_EasRecordDate(string year, string month)
        {
            _contractAllocations = new List<ContractAllocation>()
            {
                new ContractAllocation()
                {
                    FundingStreamPeriodCode = "APPS2021",
                    EndDate = new DateTime(2021, 07, 31)
                }
            };

            var easRecord = new EasCsvRecord()
            {
                AdjustmentType = "adjustment",
                FundingLine = "FundingLine2021",
                CalendarYear = year,
                CalendarMonth = month,
                Value = "10"
            };
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, null, null, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("FundingLine_02"));
        }

        [Theory]
        [InlineData("2021", "09")]
        [InlineData("2021", "08")]
        [InlineData("2021", "07")]
        [InlineData("2021", "06")]
        public void FundingLine_02_No_Error_WhenContractEndDate_IsNull(string year, string month)
        {
            _contractAllocations = new List<ContractAllocation>()
            {
                new ContractAllocation()
                {
                    FundingStreamPeriodCode = "APPS2021",
                    EndDate = null
                }
            };

            var easRecord = new EasCsvRecord()
            {
                AdjustmentType = "adjustment",
                FundingLine = "FundingLine2021",
                CalendarYear = year,
                CalendarMonth = month,
                Value = "10"
            };
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, null, null, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("FundingLine_02"));
        }

        [Fact]
        public void FundingLine_02_HaveError_When_ContractTypeIsNotFound_For_A_FundingLine()
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2020",
                Value = "1",
                FundingLine = "FundingLineWithoutContract",
                AdjustmentType = "adjustmentType"
            };

            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, null, null, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.FundingLine, easRecord).WithErrorCode("FundingLine_02");
        }

        [Theory]
        [InlineData("Adult Education - Eligible for MCA/GLA funding (non-procured)")]
        [InlineData("Adult Education - Eligible for MCA/GLA funding (procured)")]
        public void FundingLine_02_NotHaveError_When_ContractTypeIsNotRequiredForFundline(string fundline)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2020",
                Value = "1",
                FundingLine = fundline,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, null, null, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("FundingLine_02"));
        }

        [Theory]
        [InlineData("2021", "04")]
        [InlineData("2021", "07")]
        public void FundingLine_03_Error_WhenContractEndDate_OutOfRange_EasRecordDate(string year, string month)
        {
            _contractAllocations = new List<ContractAllocation>()
            {
                new ContractAllocation()
                {
                    FundingStreamPeriodCode = "APPS2021",
                    StartDate = new DateTime(2021, 05, 01),
                    EndDate = new DateTime(2021, 06, 30)
                }
            };

            var easRecord = new EasCsvRecord()
            {
                AdjustmentType = "adjustment",
                FundingLine = "FundingLine2021",
                CalendarYear = year,
                CalendarMonth = month,
                Value = "10"
            };
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, null, null, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.Contains(result.Errors, x => x.ErrorCode.Equals("FundingLine_03"));
        }


        [Theory]
        [InlineData("2021", "07")]
        [InlineData("2021", "06")]
        public void FundingLine_03_No_Error_WhenContractEndDate_InRange_EasRecordDate(string year, string month)
        {
            _contractAllocations = new List<ContractAllocation>()
            {
                new ContractAllocation()
                {
                    FundingStreamPeriodCode = "APPS2021",
                    StartDate = new DateTime(2021, 06, 01),
                    EndDate = new DateTime(2021, 07, 31)
                }
            };

            var easRecord = new EasCsvRecord()
            {
                AdjustmentType = "adjustment",
                FundingLine = "FundingLine2021",
                CalendarYear = year,
                CalendarMonth = month,
                Value = "10"
            };
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, null, null, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("FundingLine_03"));
        }

        [Theory]
        [InlineData("2021", "09")]
        [InlineData("2021", "08")]
        [InlineData("2021", "07")]
        [InlineData("2021", "06")]
        public void FundingLine_03_No_Error_WhenContractEndDate_IsNull(string year, string month)
        {
            _contractAllocations = new List<ContractAllocation>()
            {
                new ContractAllocation()
                {
                    FundingStreamPeriodCode = "APPS2021",
                    StartDate = new DateTime(2021, 06, 01),
                    EndDate = null
                }
            };

            var easRecord = new EasCsvRecord()
            {
                AdjustmentType = "adjustment",
                FundingLine = "FundingLine2021",
                CalendarYear = year,
                CalendarMonth = month,
                Value = "10"
            };
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, null, null, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("FundingLine_03"));
        }

        [Theory]
        [InlineData("Adult Education - Eligible for MCA/GLA funding (non-procured)")]
        [InlineData("Adult Education - Eligible for MCA/GLA funding (procured)")]
        public void FundingLine_03_NotHaveError_When_ContractTypeIsNotRequiredForFundline(string fundline)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "7",
                CalendarYear = "2020",
                Value = "1",
                FundingLine = fundline,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, null, null, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("FundingLine_03"));
        }
    }
}