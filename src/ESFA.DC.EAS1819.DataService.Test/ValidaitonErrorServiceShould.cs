using System.Configuration;
using ESFA.DC.EAS1819.DataService;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.EF;
using Xunit;

namespace ESFA.DC.EAS1819.Services.Test.Data
{
    using System;

    public class ValidaitonErrorServiceShould
    {
        [Fact]
        public void LogValidationError()
        {
            var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];
            IRepository<ValidationError> validationErrorRepo = new Repository<ValidationError>(context: new EasdbContext(connString));
            var submissionId = Guid.NewGuid();
            var validationError = new ValidationError
            {
                SourceFileId = 1,
                FundingLine = "16-18 Apprenticeships",
                AdjustmentType = "Excess Learning Support",
                CalendarMonth = "12",
                CalendarYear = "2018",
                CreatedOn = DateTime.UtcNow,
                ErrorMessage = "Error message here",
                RowId = Guid.NewGuid(),
                RuleId = "Filename_01",
                Severity = "E",
                Value = "10"
            };

            ValidationErrorService validationErrorService = new ValidationErrorService(validationErrorRepo);
            validationErrorService.LogValidationError(validationError);
            Assert.True(validationError.ValidationErrorId > 0);
        }
    }
}
