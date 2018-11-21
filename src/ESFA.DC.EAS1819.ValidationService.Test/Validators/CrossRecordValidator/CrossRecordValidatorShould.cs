namespace ESFA.DC.EAS1819.ValidationService.Test.Validators.CrossRecordValidator
{
    using System.Collections.Generic;
    using System.Linq;
    using ESFA.DC.EAS1819.Model;
    using Xunit;

    public class CrossRecordValidatorShould
    {
        ValidationService.Validators.CrossRecordValidator _validator;
        List<EasCsvRecord> _easRecords;

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
                    Value = "13.22"
                },
                new EasCsvRecord()
                {
                    FundingLine = "24+ Apprenticeships",
                    AdjustmentType = "Authorised Claims",
                    CalendarYear = "2019",
                    CalendarMonth = "3",
                    Value = "773.22"
                }
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
            var duplicatedRecord = new EasCsvRecord()
            {
                FundingLine = "16-18 Apprenticeships",
                AdjustmentType = "Excess Learning Support",
                CalendarYear = "2018",
                CalendarMonth = "9",
                Value = "234242.22"
            };

            _easRecords.Add(duplicatedRecord);
            _validator = new ValidationService.Validators.CrossRecordValidator();
            var result = _validator.Validate(_easRecords);
            Assert.False(result.IsValid);
            Assert.True(result?.Errors != null && result.Errors.Any(x => x.ErrorCode == $"Duplicate_01"));
            //IDictionary<List<EasCsvRecord>, int> records = (IDictionary<List<EasCsvRecord>, int>)result.Errors[0].CustomState;
            //foreach (var record in records)
            //{
            //    var easRecords = record.Key;
            //    var count = record.Value;
            //}
        }
    }
}
