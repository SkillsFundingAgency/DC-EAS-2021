using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ValidationService.Validators;
using ESFA.DC.EAS1920.EF;
using ESFA.DC.ReferenceData.FCS.Model;
using FluentValidation.TestHelper;
using Xunit;

namespace ESFA.DC.EAS.ValidationService.Test.Validators.BusinessValidator
{
    public class FundingLine02Validations : BusinessValidatorBase
    {
        [Theory]
        [InlineData("2019", "08")]
        [InlineData("2020", "01")]
        public void Have_Error_WhenContractEndDate_IsLessThan_EasRecordDate(string year, string month)
        {
            _contractAllocations = new List<ContractAllocation>()
            {
                new ContractAllocation()
                {
                    FundingStreamPeriodCode = "APPS1920",
                    EndDate = new DateTime(2019, 07, 31)
                }
            };

            var easRecord = new EasCsvRecord()
            {
                AdjustmentType = "adjustment",
                FundingLine = "FundingLine1920",
                CalendarYear = year,
                CalendarMonth = month,
                Value = "10"
            };
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.FundingLine, easRecord).WithErrorCode("FundingLine_02");
        }

        [Theory]
        [InlineData("2019", "07")]
        [InlineData("2019", "06")]
        public void No_Error_WhenContractEndDate_IsGreaterThanOrEqualTo_EasRecordDate(string year, string month)
        {
            _contractAllocations = new List<ContractAllocation>()
            {
                new ContractAllocation()
                {
                    FundingStreamPeriodCode = "APPS1920",
                    EndDate = new DateTime(2019, 07, 31)
                }
            };

            var easRecord = new EasCsvRecord()
            {
                AdjustmentType = "adjustment",
                FundingLine = "FundingLine1920",
                CalendarYear = year,
                CalendarMonth = month,
                Value = "10"
            };
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("FundingLine_02"));
        }

        [Theory]
        [InlineData("2019", "09")]
        [InlineData("2019", "08")]
        [InlineData("2019", "07")]
        [InlineData("2019", "06")]
        public void No_Error_WhenContractEndDate_IsNull(string year, string month)
        {
            _contractAllocations = new List<ContractAllocation>()
            {
                new ContractAllocation()
                {
                    FundingStreamPeriodCode = "APPS1920",
                    EndDate = null
                }
            };

            var easRecord = new EasCsvRecord()
            {
                AdjustmentType = "adjustment",
                FundingLine = "FundingLine1920",
                CalendarYear = year,
                CalendarMonth = month,
                Value = "10"
            };
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("FundingLine_02"));
        }

        [Fact]
        public void HaveError_When_ContractTypeIsNotFound_For_A_FundingLine()
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2019",
                Value = "1",
                FundingLine = "FundingLineWithoutContract",
                AdjustmentType = "adjustmentType"
            };
           
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.FundingLine, easRecord).WithErrorCode("FundingLine_02");
        }

        [Theory]
        [InlineData("Adult Education - Eligible for MCA/GLA funding (non-procured)")]
        [InlineData("Adult Education - Eligible for MCA/GLA funding (procured)")]
        public void NotHaveError_When_ContractTypeIsNotRequiredForFundline(string fundline)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2019",
                Value = "1",
                FundingLine = fundline,
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2019, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("FundingLine_02"));
        }
    }
}
