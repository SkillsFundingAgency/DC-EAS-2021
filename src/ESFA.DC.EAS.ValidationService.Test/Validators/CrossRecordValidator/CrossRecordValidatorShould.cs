using FluentValidation.TestHelper;
using ESFA.DC.EAS.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ESFA.DC.EAS.ValidationService.Test.Validators.CrossRecordValidator
{
    public class CrossRecordValidatorShould
    {
        private ValidationService.Validators.CrossRecordValidator _validator;
        private List<EasCsvRecord> _easRecords;

        public CrossRecordValidatorShould()
        {
            _easRecords = new List<EasCsvRecord>()
            {
                new EasCsvRecord()
                {
                    FundingLine = "16-18 Apprenticeships",
                    AdjustmentType = "Excess Learning Support",
                    CalendarYear = "2018",
                    CalendarMonth = "9",
                    Value = "13.22",
                    DevolvedAreaSourceOfFunding = "110",
                },
                new EasCsvRecord()
                {
                    FundingLine = "16-18 Apprenticeships",
                    AdjustmentType = "Excess Learning Support",
                    CalendarYear = "2018",
                    CalendarMonth = "10",
                    Value = "13.22",
                    DevolvedAreaSourceOfFunding = null,
                },
                new EasCsvRecord()
                {
                    FundingLine = "24+ Apprenticeships",
                    AdjustmentType = "Authorised Claims",
                    CalendarYear = "2020",
                    CalendarMonth = "3",
                    Value = "773.22",
                    DevolvedAreaSourceOfFunding = "116",
                },
                new EasCsvRecord()
                {
                    FundingLine = "24+ Apprenticeships",
                    AdjustmentType = "Authorised Claims",
                    CalendarYear = "2020",
                    CalendarMonth = "4",
                    Value = "773.22",
                    DevolvedAreaSourceOfFunding = "",
                },
            };
        }

        [Fact]
        public void NotHaveError_When_DuplicateRecords_AreNotFound()
        {
            _validator = new ValidationService.Validators.CrossRecordValidator();
            var result = _validator.Validate(_easRecords);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void HaveError_When_DuplicateRecordsFound()
        {
            var duplicatedRecords = new List<EasCsvRecord>()
            {
                new EasCsvRecord()
                {
                    FundingLine = "16-18 Apprenticeships",
                    AdjustmentType = "Excess Learning Support",
                    CalendarYear = "2018",
                    CalendarMonth = "9",
                    Value = "121.22",
                    DevolvedAreaSourceOfFunding = "110",
                },
                new EasCsvRecord()
                {
                    FundingLine = "24+ Apprenticeships",
                    AdjustmentType = "Authorised Claims",
                    CalendarYear = "2019",
                    CalendarMonth = "4",
                    Value = "4545.22",
                    DevolvedAreaSourceOfFunding = "",
                },
            };

            _easRecords.AddRange(duplicatedRecords);
            _validator = new ValidationService.Validators.CrossRecordValidator();
            _validator.ShouldHaveValidationErrorFor(x=> x.Any(), _easRecords).WithErrorCode("Duplicate_01");
        }
    }
}
