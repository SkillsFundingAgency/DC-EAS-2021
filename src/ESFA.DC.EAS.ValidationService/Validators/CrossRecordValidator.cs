namespace ESFA.DC.EAS.ValidationService.Validators
{
    using System.Collections.Generic;
    using System.Linq;
    using ESFA.DC.EAS.Common.Extensions;
    using FluentValidation;
    using Model;

    public class CrossRecordValidator : AbstractValidator<List<EasCsvRecord>>
    {
        public CrossRecordValidator()
        {
            RuleFor(x => x).Must(BeUnique)
                .WithErrorCode("Duplicate_01")
                .WithState(DuplicateRecords);
        }

        private IDictionary<List<EasCsvRecord>, int> DuplicateRecords(List<EasCsvRecord> records)
        {
           IDictionary<List<EasCsvRecord>, int> dictionary = records.GroupBy(x => new
               {
                   FundingLine = x.FundingLine.RemoveWhiteSpacesNonAlphaNumericCharacters(),
                   AdjustmentType = x.AdjustmentType.RemoveWhiteSpacesNonAlphaNumericCharacters(),
                   x.CalendarYear,
                   x.CalendarMonth,
                   x.DevolvedAreaSourceOfFunding
               })
                .Where(g => g.Count() > 1)
                .Select(group =>
                    new
                    {
                        Count = group.Count(),
                        easCscRecords = group
                    })
                .ToDictionary(kv => kv.easCscRecords.ToList(), kv => kv.Count);
            return dictionary;
        }

        private bool BeUnique(List<EasCsvRecord> records)
        {
            if (records == null)
            {
                return true;
            }

            IDictionary<List<EasCsvRecord>, int> duplicates = DuplicateRecords(records);
            return duplicates.Count == 0;
        }
    }
}
