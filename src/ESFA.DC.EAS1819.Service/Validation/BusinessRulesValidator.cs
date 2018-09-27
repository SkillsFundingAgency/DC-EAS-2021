namespace ESFA.DC.EAS1819.Service.Validation
{
    using System;
    using System.Collections.Generic;
    using ESFA.DC.DateTimeProvider.Interface;
    using ESFA.DC.EAS1819.Model;
    using FluentValidation;

    public class BusinessRulesValidator : AbstractValidator<EasCsvRecord>
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public BusinessRulesValidator(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            RuleFor(x => x.CalendarMonth)
                .NotNull()
                .InclusiveBetween(1, 12)
                .WithMessage("The Calendar Month is not valid.")
                .WithErrorCode("CalendarMonth_01");

            RuleFor(x => x.CalendarYear)
                .NotNull()
                .InclusiveBetween(2018, 2019)
                .WithMessage("The CalendarYear is not valid.")
                .WithErrorCode("CalendarYear_01");

            RuleFor(x => x).Must(NotBeInfuture)
                .WithErrorCode("CalendarYearCalendarMonth_01")
                .WithMessage("The CalendarMonth you have submitted data for cannot be in the future.");

            RuleFor(x => x.CalendarMonth).Must((easRecord, calendarMonth) => BeInTheAcademicYear(easRecord))
            .WithErrorCode("CalendarYearCalendarMonth_02")
                .WithMessage("The CalendarMonth / year you have submitted data for is not within this academic year.");
        }

        private bool BeInTheAcademicYear(EasCsvRecord record)
        {
            var inValidMonthsIn2018 = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            var inValidMonthsIn2019 = new List<int> { 8, 9, 10, 11, 12 };
            if ((record.CalendarYear == 2018 && inValidMonthsIn2018.Contains(record.CalendarMonth))
                ||
                (record.CalendarYear == 2019 && inValidMonthsIn2019.Contains(record.CalendarMonth)))
            {
                return false;
            }

            return true;
        }

        private bool NotBeInfuture(EasCsvRecord record)
        {
            if (record.CalendarMonth >= 1 && record.CalendarMonth <= 12
                &&
               (record.CalendarYear.Equals(2018) || record.CalendarYear.Equals(2019)))
            {
                var recordDateTime = new DateTime(year: record.CalendarYear, month: record.CalendarMonth, day: 1);

                if (recordDateTime > _dateTimeProvider.GetNowUtc())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
