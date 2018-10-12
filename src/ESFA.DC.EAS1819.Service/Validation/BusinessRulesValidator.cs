using ESFA.DC.EAS1819.DataService.Interface.FCS;
using ESFA.DC.ReferenceData.FCS.Model;

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

    public class BusinessRulesValidator : AbstractValidator<EasCsvRecord>
    {
        private readonly List<ContractAllocation> _contractAllocations;
        private readonly List<FundingLineContractMapping> _fundingLineContractTypeMappings;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly List<PaymentTypes> _paymentTypes;

        public BusinessRulesValidator(
            List<ContractAllocation> contractAllocations,
            List<FundingLineContractMapping> fundingLineContractTypeMappings,
            List<PaymentTypes> paymentTypes,
            IDateTimeProvider dateTimeProvider)
        {
            _contractAllocations = contractAllocations;
            _fundingLineContractTypeMappings = fundingLineContractTypeMappings;
            _dateTimeProvider = dateTimeProvider;
            _paymentTypes = paymentTypes;

            RuleFor(x => x.CalendarMonth)
                .NotNull()
                .InclusiveBetween(1, 12)
                .WithMessage("The Calendar Month is not valid.")
                .WithErrorCode("CalendarMonth_01")
                .WithState(x => x);

            RuleFor(x => x.CalendarYear)
                .NotNull()
                .InclusiveBetween(2018, 2019)
                .WithMessage("The CalendarYear is not valid.")
                .WithErrorCode("CalendarYear_01")
                .WithState(x => x);

            RuleFor(x => x.CalendarMonth).Must((easRecord, calendarMonth) => CalendarMonthAndYearMustNotBeInfuture(easRecord))
                .WithErrorCode("CalendarYearCalendarMonth_01")
                .WithMessage("The CalendarMonth you have submitted data for cannot be in the future.")
                .WithState(x => x);

            RuleFor(x => x.CalendarMonth).Must((easRecord, calendarMonth) => CalendarMonthAndYearMustBeInTheAcademicYear(easRecord))
                .WithErrorCode("CalendarYearCalendarMonth_02")
                .WithMessage("The CalendarMonth / year you have submitted data for is not within this academic year.")
                .WithState(x => x);

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

            RuleFor(x => x.Value)
                .NotEmpty()
                .WithErrorCode("Value_01")
                .WithMessage("The value field must be entered.")
                .InclusiveBetween((decimal)-99999999.99, (decimal)99999999.99)
                .WithErrorCode("Value_03")
                .WithMessage("Value must be >=-99999999.99 and <=99999999.99")
                .WithState(x => x);
        }

        private bool FundingLineMustHaveValidContractType(string fundingLine)
        {
            if (_fundingLineContractTypeMappings != null)
            {
                var contractTypesRequired = _fundingLineContractTypeMappings.
                    Where(x => x.FundingLine.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower().Equals(fundingLine.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower()))
                    .Select(x => x.ContractTypeRequired)
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
                     x.AdjustmentType.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower().
                         Equals(easRecord.AdjustmentType.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower())
                         &&
                         (x.FundingLine != null &&
                             x.FundingLine.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower().
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
                     x.AdjustmentType.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower().
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
                     x.FundingLine.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower().
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
            if ((record.CalendarYear == 2018 && inValidMonthsIn2018.Contains(record.CalendarMonth))
                ||
                (record.CalendarYear == 2019 && inValidMonthsIn2019.Contains(record.CalendarMonth)))
            {
                return false;
            }

            return true;
        }

        private bool CalendarMonthAndYearMustNotBeInfuture(EasCsvRecord record)
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
