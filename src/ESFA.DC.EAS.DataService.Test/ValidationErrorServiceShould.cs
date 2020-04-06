using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1920.EF;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ESFA.DC.EAS.DataService.Test
{
    using System;

    public class ValidationErrorServiceShould
    {
        [Fact]
        public async Task LogErrorSourceFileAndValidationErrors()
        {
            var connString = ConfigurationManager.AppSettings["EasdbConnectionString"];
            DbContextOptions<EasContext> options = new DbContextOptionsBuilder<EasContext>().UseSqlServer(connString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            EasContext easdbContext = new EasContext(options);
            ValidationErrorService validationErrorService = new ValidationErrorService(easdbContext, new SeriLogger(new ApplicationLoggerSettings(), new Logging.ExecutionContext(), null));

            var sourceFile = new SourceFile()
            {
                DateTime = DateTime.UtcNow,
                FileName = "EAS-10033670-1819-20180812-100221-05",
                FilePreparationDate = DateTime.UtcNow.AddHours(-1),
                Ukprn = "10033670"
            };

            int sourceFileId = await validationErrorService.LogErrorSourceFileAsync(sourceFile, CancellationToken.None);

            var validationError = new ValidationError
            {
                FundingLine = "16-18 Apprenticeships",
                AdjustmentType = "Excess Learning Support",
                CalendarMonth = "12",
                CalendarYear = "2018",
                CreatedOn = DateTime.UtcNow,
                ErrorMessage = "Error message here",
                RowId = Guid.NewGuid(),
                RuleId = "Filename_01",
                Severity = "E",
                Value = "10",
                DevolvedAreaSoF = "116",
                SourceFileId = sourceFileId
            };

            await validationErrorService.LogValidationErrorsAsync(new List<ValidationError> { validationError }, CancellationToken.None);
            Assert.True(validationError.ValidationErrorId > 0);
        }
    }
}
