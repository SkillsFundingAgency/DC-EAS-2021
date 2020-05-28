using ESFA.DC.EAS.ValidationService.Validators;

namespace ESFA.DC.EAS.ValidationService.Test.Validators.BusinessValidator
{
    using System;
    using System.Linq;
    using ESFA.DC.DateTimeProvider.Interface;
    using ESFA.DC.EAS.Model;
    using FluentValidation.TestHelper;
    using Moq;
    using Xunit;

    public partial class CalendarMonthYearValidations : BusinessValidatorBase
    {
        [Theory]
        [InlineData("-1")]
        [InlineData("20000")]
        [InlineData("3234242")]
        [InlineData("0")]
        [InlineData("13")]
        [InlineData("$")]
        [InlineData("£")]
        [InlineData(null)]
        [InlineData("")]
        public void HaveError_WhenCalendarMonth_Is_NotValid(string calendarMonth)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = calendarMonth,
                CalendarYear = "2020",
                Value = "1",
                AdjustmentType = "AdjustmentType",
                FundingLine = "FundingLine"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 09, 01));
            _validator = new BusinessRulesValidator(null, null, paymentTypes, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.CalendarMonth, easRecord).WithErrorCode("CalendarMonth_01");
        }

        [Fact]
        public void Not_HaveAnError_WhenCalendarMonth_Is_Valid()
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2020",
                Value = "1",
                AdjustmentType = "AdjustmentType",
                FundingLine = "FundingLine"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("-1")]
        [InlineData("20000")]
        [InlineData("3234242")]
        [InlineData("0")]
        [InlineData("2022")]
        [InlineData("2018")]
        [InlineData("$")]
        [InlineData("£")]
        [InlineData(null)]
        [InlineData("")]
        public void HaveError_WhenCalendarYear_Is_NotValid(string calendarYear)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "12",
                CalendarYear = calendarYear,
                Value = "1",
                AdjustmentType = "AdjustmentType",
                FundingLine = "FundingLine"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 09, 01));
            _validator = new BusinessRulesValidator(null, null, paymentTypes, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.CalendarYear, easRecord).WithErrorCode("CalendarYear_01");
            var result = _validator.Validate(easRecord);
            //Assert.False(result.IsValid);
            //Assert.Equal("The CalendarYear is not valid.", result.Errors[0].ErrorMessage);
            //Assert.Equal("CalendarYear_01", result.Errors[0].ErrorCode);
        }

        [Fact]
        public void Not_HaveAnError_WhenCalendarYear_Is_Valid()
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2020",
                Value = "1",
                AdjustmentType = "AdjustmentType",
                FundingLine = "FundingLine"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void HaveErrorWhenCalendarMonthAndYearAreInFuture()
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "12",
                CalendarYear = "2020",
                Value = "1",
                AdjustmentType = "AdjustmentType",
                FundingLine = "FundingLine"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 09, 01));
            _validator = new BusinessRulesValidator(null, null, paymentTypes, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.CalendarMonth, easRecord)
                .WithErrorCode("CalendarYearCalendarMonth_01");
        }

        [Fact]
        public void NotHaveError_When_CalendarMonth_And_Year_Are_NotInFuture()
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = "8",
                CalendarYear = "2020",
                Value = "1",
                AdjustmentType = "AdjustmentType",
                FundingLine = "FundingLine"
            };
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2020, 09, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("1", "2020")]
        [InlineData("2", "2020")]
        [InlineData("3", "2020")]
        [InlineData("4", "2020")]
        [InlineData("5", "2020")]
        [InlineData("6", "2020")]
        [InlineData("7", "2020")]
        [InlineData("8", "2021")]
        [InlineData("9", "2021")]
        [InlineData("10", "2021")]
        [InlineData("11", "2021")]
        [InlineData("12", "2021")]
        public void HaveErrorWhenCalendarMonthAndYearAreNotInTheAcademicYear(string calendarMonth, string calendarYear)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = calendarMonth,
                CalendarYear = calendarYear,
                Value = "1",
                AdjustmentType = "AdjustmentType",
                FundingLine = "FundingLine"
            };
            // Mock future date , so that validation doesn't fail on Calendar Month future date
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2021, 10, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            _validator.ShouldHaveValidationErrorFor(x => x.CalendarMonth, easRecord).WithErrorCode("CalendarYearCalendarMonth_02");
        }

        [Theory]
        [InlineData("8", "2020")]
        [InlineData("9", "2020")]
        [InlineData("10", "2020")]
        [InlineData("11", "2020")]
        [InlineData("12", "2020")]
        [InlineData("1", "2021")]
        [InlineData("2", "2021")]
        [InlineData("3", "2021")]
        [InlineData("4", "2021")]
        [InlineData("5", "2021")]
        [InlineData("6", "2021")]
        [InlineData("7", "2021")]

        public void NotHaveErrorWhenCalendarMonthAndYearAreInTheAcademicYear(string calendarMonth, string calendarYear)
        {
            var easRecord = new EasCsvRecord()
            {
                CalendarMonth = calendarMonth,
                CalendarYear = calendarYear,
                Value = "1",
                AdjustmentType = "AdjustmentType",
                FundingLine = "FundingLine"
            };
            // Mock future date , so that validation doesn't fail on Calendar Month future date
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2021, 10, 01));
            _validator = new BusinessRulesValidator(_contractAllocations, _fundingLineContractTypeMappings, paymentTypes, dateTimeProviderMock.Object, 1);
            var result = _validator.Validate(easRecord);
            Assert.False(result?.Errors != null && result.Errors.Any(x => x.ErrorCode == $"CalendarYearCalendarMonth_02"));
        }
    }
}
