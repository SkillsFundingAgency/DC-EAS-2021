using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using ESFA.DC.ReferenceData.FCS.Model;

namespace ESFA.DC.EAS1819.ValidationService.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ESFA.DC.DateTimeProvider.Interface;
    using ESFA.DC.EAS1819.EF;
    using ESFA.DC.EAS1819.Model;
    using ESFA.DC.EAS1819.ValidationService.Extensions;
    using FluentValidation;

    public class BusinessRulesValidator : AbstractValidator<EasCsvRecord>
    {
        private readonly List<ContractAllocation> _contractAllocations;
        private readonly List<FundingLineContractTypeMapping> _fundingLineContractTypeMappings;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly List<PaymentTypes> _paymentTypes;

        public BusinessRulesValidator(
            List<ContractAllocation> contractAllocations,
            List<FundingLineContractTypeMapping> fundingLineContractTypeMappings,
            List<PaymentTypes> paymentTypes,
            IDateTimeProvider dateTimeProvider)
        {
            _contractAllocations = contractAllocations;
            _fundingLineContractTypeMappings = fundingLineContractTypeMappings;
            _dateTimeProvider = dateTimeProvider;
            _paymentTypes = paymentTypes;

            RuleFor(x => x.CalendarMonth).Must(BeAValidMonth)
                .WithMessage("The Calendar Month is not valid.")
                .WithErrorCode("CalendarMonth_01")
                .WithState(x => x);

            RuleFor(x => x.CalendarYear).Must(BeAValidYear)
                .WithMessage("The CalendarYear is not valid.")
                .WithErrorCode("CalendarYear_01")
                .WithState(x => x);

            When(x => BeAValidMonth(x.CalendarMonth) && BeAValidYear(x.CalendarYear), () =>
            {
                RuleFor(x => x.CalendarMonth)
                    .Must((easRecord, calendarMonth) => CalendarMonthAndYearMustNotBeInfuture(easRecord))
                    .WithErrorCode("CalendarYearCalendarMonth_01")
                    .WithMessage("The CalendarMonth you have submitted data for cannot be in the future.")
                    .WithState(x => x);

                RuleFor(x => x.CalendarMonth).Must((easRecord, calendarMonth) => CalendarMonthAndYearMustBeInTheAcademicYear(easRecord))
                    .WithErrorCode("CalendarYearCalendarMonth_02")
                    .WithMessage("The CalendarMonth / year you have submitted data for is not within this academic year.")
                    .WithState(x => x);
            });

            RuleFor(x => x.FundingLine).Must(FundingLineMustBeAValidLookUp)
                .WithErrorCode("FundingLine_01")
                .WithMessage("The FundingLine is not valid.")
                .WithState(x => x);

            RuleFor(x => x.FundingLine).Must(FundingLineMustHaveValidContractType)
                .WithErrorCode("FundingLine_02")
                .WithMessage("To claim earning adjustments against funding lines, an appropriate contract type must be held.")
                .WithState(x => x);

            RuleFor(x => x.AdjustmentType).Must(AdjustmentTypeMustBeAValidLookUp)
                .WithErrorCode("AdjustmentType_01")
                .WithMessage("The AdjustmentType must be a valid lookup.")
                .WithState(x => x);

            RuleFor(x => x.AdjustmentType).Must((easRecord, calendarMonth) => AdjustmentTypeValidFortheGivenFundingLine(easRecord))
                .WithErrorCode("AdjustmentType_02")
                .WithMessage("The AdjustmentType must be valid for the type of funding line returned.")
                .WithState(x => x);

            RuleFor(x => x.Value).Cascade(CascadeMode.StopOnFirstFailure).Must(BeAValidValue)
                .WithErrorCode("Value_01")
                .WithMessage("The value field must be entered as a non-zero numerical value.")
                .WithState(x => x)
                .Must(BeWithInTheRange)
                .WithErrorCode("Value_03")
                .WithMessage("Value must be >=-99999999.99 and <=99999999.99")
                .WithState(x => x);
        }

        private bool BeAValidYear(string calendarYear)
        {
            var validYears = new List<int> { 2018, 2019 };

            int result;
            if (string.IsNullOrEmpty(calendarYear))
            {
                return false;
            }

            bool canParse = int.TryParse(calendarYear, out result);
            if (!canParse)
            {
                return false;
            }

            if (!validYears.Contains(result))
            {
                return false;
            }

            return true;
        }

        private bool BeAValidMonth(string calendarMonth)
        {
            int result;
            if (string.IsNullOrEmpty(calendarMonth))
            {
                return false;
            }

            bool canParse = int.TryParse(calendarMonth, out result);
            if (!canParse)
            {
                return false;
            }

            if (result < 1 || result > 12)
            {
                return false;
            }

            return true;
        }

        private bool BeWithInTheRange(string value)
        {
            decimal result;
            bool canParse = decimal.TryParse(value, out result);
            if (result < (decimal)-99999999.99 || result > (decimal)99999999.99)
            {
                return false;
            }

            return true;
        }

        private bool BeAValidValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            bool canParse = decimal.TryParse(value, out decimal result);
            if (!canParse)
            {
                return false;
            }

            return true;
        }

        private bool FundingLineMustHaveValidContractType(string fundingLine)
        {
            if (_fundingLineContractTypeMappings != null)
            {
                var contractTypesRequired = _fundingLineContractTypeMappings.
                    Where(x => x.FundingLine.Name.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower().Equals(fundingLine.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower()))
                    .Select(x => x.ContractType.Name)
                    .Distinct().ToList();
                if (_contractAllocations != null && (contractTypesRequired.Count > 0
                                                     && _contractAllocations.Any(x => contractTypesRequired.Contains(x.FundingStreamPeriodCode))))
                {
                    return true;
                }
            }

            return false;
        }

        private bool AdjustmentTypeValidFortheGivenFundingLine(EasCsvRecord easRecord)
        {
            var paymentType = _paymentTypes.FirstOrDefault(
                x => x.AdjustmentType != null &&
                     x.AdjustmentType.Name.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower().
                         Equals(easRecord.AdjustmentType.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower())
                         &&
                         (x.FundingLine != null &&
                             x.FundingLine.Name.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower().
                                 Equals(easRecord.FundingLine.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower())));
            if (paymentType == null)
            {
                return false;
            }

            return true;
        }

        private bool AdjustmentTypeMustBeAValidLookUp(string adjustmentType)
        {
            if (string.IsNullOrEmpty(adjustmentType))
            {
                return false;
            }

            var paymentType = _paymentTypes.FirstOrDefault(
                x => x.AdjustmentType != null &&
                     x.AdjustmentType.Name.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower().
                         Equals(adjustmentType.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower()));
            if (paymentType == null)
            {
                return false;
            }

            return true;
        }

        private bool FundingLineMustBeAValidLookUp(string fundingLine)
        {
            if (string.IsNullOrEmpty(fundingLine))
            {
                return false;
            }

            var paymentType = _paymentTypes.FirstOrDefault(
                x => x.FundingLine != null &&
                     x.FundingLine.Name.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower().
                         Equals(fundingLine.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower()));
            if (paymentType == null)
            {
                return false;
            }

            return true;
        }

        private bool CalendarMonthAndYearMustBeInTheAcademicYear(EasCsvRecord record)
        {
            var inValidMonthsIn2018 = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            var inValidMonthsIn2019 = new List<int> { 8, 9, 10, 11, 12 };
            if ((Convert.ToInt32(record.CalendarYear) == 2018 && inValidMonthsIn2018.Contains(Convert.ToInt32(record.CalendarMonth)))
                ||
                (Convert.ToInt32(record.CalendarYear) == 2019 && inValidMonthsIn2019.Contains(Convert.ToInt32(record.CalendarMonth))))
            {
                return false;
            }

            return true;
        }

        private bool CalendarMonthAndYearMustNotBeInfuture(EasCsvRecord record)
        {
            if (Convert.ToInt32(record.CalendarMonth) >= 1 && Convert.ToInt32(record.CalendarMonth) <= 12
                &&
               (record.CalendarYear.Equals("2018") || record.CalendarYear.Equals("2019")))
            {
                var recordDateTime = new DateTime(year: Convert.ToInt32(record.CalendarYear), month: Convert.ToInt32(record.CalendarMonth), day: 1);

                if (recordDateTime > _dateTimeProvider.GetNowUtc())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
