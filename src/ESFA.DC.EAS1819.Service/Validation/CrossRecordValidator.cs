using System.Security.Cryptography.X509Certificates;

namespace ESFA.DC.EAS1819.Service.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ESFA.DC.DateTimeProvider.Interface;
    using ESFA.DC.EAS1819.EF;
    using ESFA.DC.EAS1819.Model;
    using ESFA.DC.EAS1819.Service.Extensions;
    using FluentValidation;

    public class CrossRecordValidator : AbstractValidator<List<EasCsvRecord>>
    {
        public CrossRecordValidator()
        {
            RuleFor(x => x).Must(BeUnique)
                .WithErrorCode("Duplicate_01")
                .WithMessage("This record is a duplicate.")
                .WithState(DuplicateRecords);
        }

        private IDictionary<List<EasCsvRecord>, int> DuplicateRecords(List<EasCsvRecord> records)
        {
           IDictionary< List<EasCsvRecord>, int> dictionary = records.GroupBy(x => new { x.FundingLine, x.AdjustmentType, x.CalendarMonth })
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

            var duplicates = DuplicateRecords(records);
            if (duplicates.Count > 0)
            {
               return false;
            }

            return true;
        }
    }
}
