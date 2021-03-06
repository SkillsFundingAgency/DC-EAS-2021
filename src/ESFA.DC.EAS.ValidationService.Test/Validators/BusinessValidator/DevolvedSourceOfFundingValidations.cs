﻿using System;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.ValidationService.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace ESFA.DC.EAS.ValidationService.Test.Validators.BusinessValidator
{
    public class DevolvedSourceOfFundingValidations : BusinessValidatorBase
    {
        private EasCsvRecord easRecord;

        public DevolvedSourceOfFundingValidations()
        {
            easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2020",
                Value = "1",
                AdjustmentType = "adjustmentType"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 09, 01));
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-10")]
        public void HaveError_When_NotAValidLookUp(string devolvedAreaSourceOfFunding)
        {
            easRecord.FundingLine = "fundingLine";
            easRecord.DevolvedAreaSourceOfFunding = devolvedAreaSourceOfFunding;
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, _devolvedContracts, _sofCodeDictionary, dateTimeProviderMock.Object, 1);
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
        [InlineData("117")]
        public void NotHaveError_When_DevolvedAreaSourceOfFunding_IsValid(string devolvedAreaSourceOfFunding)
        {
            easRecord.FundingLine = "fundingLine";
            easRecord.DevolvedAreaSourceOfFunding = devolvedAreaSourceOfFunding;
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, _devolvedContracts, _sofCodeDictionary, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("DevolvedAreaSourceOfFunding_03"));
        }


        [Theory]
        [InlineData("Adult Education - Eligible for MCA/GLA funding (non-procured)")]
        [InlineData("Adult Education - Eligible for MCA/GLA funding (procured)")]
        public void HaveError_When_DevolvedSourceOfFundingDoesNotExist_For_Required_Fundlines(string fundingLine)
        {
            easRecord.FundingLine = fundingLine;
            easRecord.DevolvedAreaSourceOfFunding = null;
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, _devolvedContracts, _sofCodeDictionary, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.DevolvedAreaSourceOfFunding, easRecord).WithErrorCode("DevolvedAreaSourceOfFunding_01");
        }

        [Theory]
        [InlineData("Adult Education - Eligible for MCA/GLA funding (non-procured)", "110")]
        [InlineData("Adult Education - Eligible for MCA/GLA funding (procured)", "116")]
        public void NotHaveError_When_DevolvedSourceOfFundingExist_For_Required_Fundlines(string fundingLine, string devolvedSourceOfFunding)
        {
            easRecord.FundingLine = fundingLine;
            easRecord.DevolvedAreaSourceOfFunding = devolvedSourceOfFunding;
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, _devolvedContracts, _sofCodeDictionary, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("DevolvedAreaSourceOfFunding_01"));
        }

        [Theory]
        [InlineData("Fundingline1", "100")]
        [InlineData("Fundingline2", "116")]
        public void HaveError_When_DevolvedSourceOfFundingExist_For_NonRequired_Fundlines(string fundingLine, string devolvedSourceOfFunding)
        {
            easRecord.FundingLine = fundingLine;
            easRecord.DevolvedAreaSourceOfFunding = devolvedSourceOfFunding;
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, _devolvedContracts, _sofCodeDictionary, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.DevolvedAreaSourceOfFunding, easRecord).WithErrorCode("DevolvedAreaSourceOfFunding_02");
        }

        [Theory]
        [InlineData("Fundingline1", null)]
        [InlineData("Fundingline2", "")]
        public void NotHaveError_When_DevolvedSourceOfFunding_DoesNotExist_For_NonRequired_Fundlines(string fundingLine, string devolvedSourceOfFunding)
        {
            easRecord.FundingLine = fundingLine;
            easRecord.DevolvedAreaSourceOfFunding = devolvedSourceOfFunding;
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, _devolvedContracts, _sofCodeDictionary, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("DevolvedAreaSourceOfFunding_02"));
        }

        [Fact]
        public void DevolvedAreaSourceOfFunding_04_Error_NoContractsFound()
        {
            easRecord.FundingLine = "Adult Education -Eligible for MCA / GLA funding(non - procured)";
            easRecord.DevolvedAreaSourceOfFunding = "114";
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, _devolvedContracts, _sofCodeDictionary, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.DevolvedAreaSourceOfFunding, easRecord).WithErrorCode("DevolvedAreaSourceOfFunding_04");
        }

        [Fact]
        public void DevolvedAreaSourceOfFunding_04_Error_ContractInvalid()
        {
            easRecord.FundingLine = "Adult Education - Eligible for MCA/GLA funding (non-procured)";
            easRecord.DevolvedAreaSourceOfFunding = "116";
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, _devolvedContracts, _sofCodeDictionary, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.DevolvedAreaSourceOfFunding, easRecord).WithErrorCode("DevolvedAreaSourceOfFunding_04");
        }

        [Fact]
        public void DevolvedAreaSourceOfFunding_04_NoError_ContractValid()
        {
            easRecord.FundingLine = "Adult Education - Eligible for MCA/GLA funding (non-procured)";
            easRecord.CalendarMonth = "10";
            easRecord.DevolvedAreaSourceOfFunding = "116";
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, _devolvedContracts, _sofCodeDictionary, dateTimeProviderMock.Object, 1);

            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("DevolvedAreaSourceOfFunding_04"));
        }

        [Fact]
        public void DevolvedAreaSourceOfFunding_04_NoError_ContractValid_MultipleContracts()
        {
            easRecord.FundingLine = "Adult Education - Eligible for MCA/GLA funding (non-procured)";
            easRecord.CalendarMonth = "1";
            easRecord.CalendarYear = "2020";
            easRecord.DevolvedAreaSourceOfFunding = "112";
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, _devolvedContracts, _sofCodeDictionary, dateTimeProviderMock.Object, 1);

            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("DevolvedAreaSourceOfFunding_04"));
        }

        [Fact]
        public void DevolvedAreaSourceOfFunding_04_NoError_ContractValidSpanningCalendarYears()
        {
            easRecord.FundingLine = "Adult Education - Eligible for MCA/GLA funding (non-procured)";
            easRecord.CalendarMonth = "8";
            easRecord.CalendarYear = "2020";
            easRecord.DevolvedAreaSourceOfFunding = "110";
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, _devolvedContracts, _sofCodeDictionary, dateTimeProviderMock.Object, 1);

            var result = _validator.Validate(easRecord);
            Assert.DoesNotContain(result.Errors, x => x.ErrorCode.Equals("DevolvedAreaSourceOfFunding_04"));
        }
    }
}