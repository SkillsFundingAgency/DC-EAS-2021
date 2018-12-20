using System.Linq;
using ESFA.DC.EAS1819.ValidationService.Validators;
using Xunit;

namespace ESFA.DC.EAS1819.ValidationService.Test.Validators
{
    public class FileValidatorShould
    {
        private readonly FileValidator _fileValidator;

        public FileValidatorShould()
        {
            _fileValidator = new FileValidator();
        }

        [Fact]
        public void NotHaveAnyErrorWhenHeadersAreValidAndInRightOrder()
        {
            string[] correctHeaders = { "FundingLine", "AdjustmentType", "CalendarYear", "CalendarMonth", "Value" };
            var result = _fileValidator.Validate(correctHeaders);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void HaveErrorForInvalidHeaders()
        {
            string[] invalidHeaders = { "asdfgLine" };
            var result = _fileValidator.Validate(invalidHeaders);
            Assert.False(result.IsValid);
            Assert.Equal("The file format is incorrect.  Please check the field headers are as per the Guidance document.", result.Errors.Single().ErrorMessage);
        }

        [Fact]
        public void HaveErrorWhenHeadersAreInWrongOrder()
        {
            string[] headersInWrongOrder = { "AdjustmentType", "CalendarYear", "CalendarMonth", "Value", "FundingLine" };
            var result = _fileValidator.Validate(headersInWrongOrder);
            Assert.False(result.IsValid);
            Assert.Equal("The file format is incorrect.  Please check the field headers are as per the Guidance document.", result.Errors.Single().ErrorMessage);
        }
    }
}
