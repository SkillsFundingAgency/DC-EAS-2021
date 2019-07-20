using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.Common.Extensions;
using ESFA.DC.EAS.Common.Helpers;
using ESFA.DC.EAS1920.EF;
using ESFA.DC.EAS.Model;
using ESFA.DC.ReferenceData.FCS.Model;
using FluentValidation;

namespace ESFA.DC.EAS.ValidationService.Validators
{
    public class BusinessRulesValidator : AbstractValidator<EasCsvRecord>
    {
        private readonly List<ContractAllocation> _contractAllocations;
        private readonly List<FundingLineContractTypeMapping> _fundingLineContractTypeMappings;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly int _returnPeriod;
        private readonly List<PaymentType> _paymentTypes;
        private readonly IEnumerable<string> _fundingLineContractTypesNotRequired = new HashSet<string>()
        {
            "Adult Education - Eligible for MCA/GLA funding (non-procured)",
            "Adult Education - Eligible for MCA/GLA funding (procured)"
        };

        public BusinessRulesValidator(
            List<ContractAllocation> contractAllocations,
            List<FundingLineContractTypeMapping> fundingLineContractTypeMappings,
            List<PaymentType> paymentTypes,
            IDateTimeProvider dateTimeProvider,
            int returnPeriod)
        {
            _contractAllocations = contractAllocations;
            _fundingLineContractTypeMappings = fundingLineContractTypeMappings;
            _dateTimeProvider = dateTimeProvider;
            _returnPeriod = returnPeriod;
            _paymentTypes = paymentTypes;

            RuleFor(x => x.CalendarMonth).Must(BeAValidMonth)
                .WithErrorCode("CalendarMonth_01")
                .WithState(x => x);

            RuleFor(x => x.CalendarYear).Must(BeAValidYear)
                .WithErrorCode("CalendarYear_01")
                .WithState(x => x);

            When(x => BeAValidMonth(x.CalendarMonth) && BeAValidYear(x.CalendarYear), () =>
            {
                RuleFor(x => x.CalendarMonth).Must((easRecord, calendarMonth) => CalendarMonthAndYearMustBeInTheAcademicYear(easRecord))
                    .WithErrorCode("CalendarYearCalendarMonth_02")
                    .WithState(x => x);
            });

            When(x => BeAValidMonth(x.CalendarMonth) && BeAValidYear(x.CalendarYear) && CalendarMonthAndYearMustBeInTheAcademicYear(x), () =>
            {
                RuleFor(x => x.CalendarMonth)
                    .Must((easRecord, calendarMonth) => CalendarMonthAndYearMustNotBeInfuture(easRecord))
                    .WithErrorCode("CalendarYearCalendarMonth_01")
                    .WithState(x => x);
            });

            RuleFor(x => x.FundingLine).Must(FundingLineMustBeAValidLookUp)
                .WithErrorCode("FundingLine_01")
                .WithState(x => x);

            RuleFor(x => x.FundingLine).Must(FundingLineMustHaveValidContractType)
                .WithErrorCode("FundingLine_02")
                .WithState(x => x);

            RuleFor(x => x.AdjustmentType).Must(AdjustmentTypeMustBeAValidLookUp)
                .WithErrorCode("AdjustmentType_01")
                .WithState(x => x);

            RuleFor(x => x.AdjustmentType).Must((easRecord, calendarMonth) => AdjustmentTypeValidFortheGivenFundingLine(easRecord))
                .WithErrorCode("AdjustmentType_02")
                .WithState(x => x);

            RuleFor(x => x.Value).Cascade(CascadeMode.StopOnFirstFailure).Must(BeAValidValue)
                .WithErrorCode("Value_01")
                .WithState(x => x)
                .Must(BeWithInTheRange)
                .WithErrorCode("Value_03")
                .WithState(x => x);
        }

        private bool BeAValidYear(string calendarYear)
        {
            var validYears = new List<int> { 2019, 2020 };

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
            if (string.IsNullOrEmpty(fundingLine))
            {
                return false;
            }

            if(_fundingLineContractTypesNotRequired.Any(x => x.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower().Equals(fundingLine.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower())))
            {
                return true;
            }

            if (_fundingLineContractTypeMappings != null)
             {
                var contractTypesRequired = _fundingLineContractTypeMappings.
                    Where(x => x.FundingLine.Name.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower().Equals(fundingLine.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower()))
                    .Select(x => x.ContractType.Name)
                    .Distinct().ToList();
                if (_contractAllocations != null 
                    && (contractTypesRequired.Count > 0
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
            var inValidMonthsIn2019 = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            var inValidMonthsIn2020 = new List<int> { 8, 9, 10, 11, 12 };
            if ((Convert.ToInt32(record.CalendarYear) == 2019 && inValidMonthsIn2019.Contains(Convert.ToInt32(record.CalendarMonth)))
                ||
                (Convert.ToInt32(record.CalendarYear) == 2020 && inValidMonthsIn2020.Contains(Convert.ToInt32(record.CalendarMonth))))
            {
                return false;
            }

            return true;
        }

        private bool CalendarMonthAndYearMustNotBeInfuture(EasCsvRecord record)
        {
            var collectionPeriod = CollectionPeriodHelper.GetCollectionPeriod(Convert.ToInt32(record.CalendarYear), Convert.ToInt32(record.CalendarMonth));
            if (collectionPeriod > _returnPeriod)
            {
                return false;
            }

            return true;
        }
    }
}
