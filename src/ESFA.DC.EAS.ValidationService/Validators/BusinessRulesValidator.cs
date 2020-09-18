using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.Common.Extensions;
using ESFA.DC.EAS.Common.Helpers;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.EAS.Model;
using ESFA.DC.ReferenceData.FCS.Model;
using FluentValidation;
using ESFA.DC.EAS.DataService.Constants;
using ESFA.DC.EAS.Interface.Constants;

namespace ESFA.DC.EAS.ValidationService.Validators
{
    public class BusinessRulesValidator : AbstractValidator<EasCsvRecord>
    {
        private readonly List<ContractAllocation> _contractAllocations;
        private readonly List<FundingLineContractTypeMapping> _fundingLineContractTypeMappings;
        private readonly IReadOnlyDictionary<string, IEnumerable<DevolvedContract>> _devolvedContracts;
        private readonly IReadOnlyDictionary<int, string> _mcaShortCodeDictionary;
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
            IReadOnlyDictionary<string, IEnumerable<DevolvedContract>> devolvedContracts,
            IReadOnlyDictionary<int, string> mcaShortCodeDictionary,
            IDateTimeProvider dateTimeProvider,
            int returnPeriod)
        {
            _contractAllocations = contractAllocations;
            _fundingLineContractTypeMappings = fundingLineContractTypeMappings;
            _devolvedContracts = devolvedContracts;
            _mcaShortCodeDictionary = mcaShortCodeDictionary;
            _dateTimeProvider = dateTimeProvider;
            _returnPeriod = returnPeriod;
            _paymentTypes = paymentTypes;

            RuleFor(x => x.CalendarMonth).Must(BeAValidMonth)
                .WithErrorCode(ErrorNameConstants.CalendarMonth_01)
                .WithState(x => x);

            RuleFor(x => x.CalendarYear).Must(BeAValidYear)
                .WithErrorCode(ErrorNameConstants.CalendarYear_01)
                .WithState(x => x);

            When(x => BeAValidMonth(x.CalendarMonth) && BeAValidYear(x.CalendarYear), () =>
            {
                RuleFor(x => x.CalendarMonth).Must((easRecord, calendarMonth) => CalendarMonthAndYearMustBeInTheAcademicYear(easRecord))
                    .WithErrorCode(ErrorNameConstants.CalendarYearCalendarMonth_02)
                    .WithState(x => x);
            });

            When(x => BeAValidMonth(x.CalendarMonth) && BeAValidYear(x.CalendarYear) && CalendarMonthAndYearMustBeInTheAcademicYear(x), () =>
            {
                RuleFor(x => x.CalendarMonth)
                    .Must((easRecord, calendarMonth) => CalendarMonthAndYearMustNotBeInfuture(easRecord))
                    .WithErrorCode(ErrorNameConstants.CalendarYearCalendarMonth_01)
                    .WithState(x => x);
            });

            RuleFor(x => x.FundingLine).Must(FundingLineMustBeAValidLookUp)
                .WithErrorCode(ErrorNameConstants.FundingLine_01)
                .WithState(x => x);

            When(x => BeAValidMonth(x.CalendarMonth) && BeAValidYear(x.CalendarYear) && CalendarMonthAndYearMustBeInTheAcademicYear(x),
                () =>
                {
                    RuleFor(x => x.FundingLine).Must((easRecord, calendarMonth) =>
                            FundingLineMustHaveValidContractType(easRecord))
                        .WithErrorCode(ErrorNameConstants.FundingLine_02)
                        .WithState(x => x);
                });

            When(x => BeAValidMonth(x.CalendarMonth) 
                      && BeAValidYear(x.CalendarYear) 
                      && CalendarMonthAndYearMustBeInTheAcademicYear(x)
                      && FundingLineMustHaveValidContractType(x),
                () =>
                {
                    RuleFor(x => x.CalendarMonth).Must((easRecord, calendarMonth) =>
                            FundingLineMustHaveValidDates(easRecord))
                        .WithErrorCode(ErrorNameConstants.FundingLine_03)
                        .WithState(x => x);
                });

            RuleFor(x => x.AdjustmentType).Must(AdjustmentTypeMustBeAValidLookUp)
                .WithErrorCode(ErrorNameConstants.AdjustmentType_01)
                .WithState(x => x);

            RuleFor(x => x.AdjustmentType).Must((easRecord, calendarMonth) => AdjustmentTypeValidFortheGivenFundingLine(easRecord))
                .WithErrorCode(ErrorNameConstants.AdjustmentType_02)
                .WithState(x => x);

            RuleFor(x => x.DevolvedAreaSourceOfFunding).Must((easRecord, devolvedAreaSourceOfFunding) => DevolvedAreaSourceOfFundingMustExist(easRecord))
                .WithErrorCode(ErrorNameConstants.DevolvedAreaSourceOfFunding_01)
                .WithState(x => x);

            RuleFor(x => x.DevolvedAreaSourceOfFunding).Must((easRecord, devolvedAreaSourceOfFunding) => DevolvedAreaSourceOfFundingMustNotExist(easRecord))
                .WithErrorCode(ErrorNameConstants.DevolvedAreaSourceOfFunding_02)
                .WithState(x => x);

            RuleFor(x => x.DevolvedAreaSourceOfFunding).Must(DevolvedAreaSourceOfFundingMustBeAValidLookUp)
                .WithErrorCode(ErrorNameConstants.DevolvedAreaSourceOfFunding_03)
                .WithState(x => x);

            RuleFor(x => x.DevolvedAreaSourceOfFunding).Must((easRecord, devolvedAreaSourceOfFunding) => DevolvedAreaSourceOfFundingMustHaveAValidDevolvedContract(easRecord))
             .WithErrorCode(ErrorNameConstants.DevolvedAreaSourceOfFunding_04)
             .WithState(x => x);

            RuleFor(x => x.Value).Cascade(CascadeMode.StopOnFirstFailure).Must(BeAValidValue)
                .WithErrorCode(ErrorNameConstants.Value_01)
                .WithState(x => x)
                .Must(BeWithInTheRange)
                .WithErrorCode(ErrorNameConstants.Value_03)
                .WithState(x => x);
        }

        public bool FundingLineMustHaveValidContractType(EasCsvRecord easRecord)
        {
            if (string.IsNullOrEmpty(easRecord.FundingLine))
            {
                return false;
            }

            if (_fundingLineContractTypesNotRequired.Any(x => x.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower().Equals(easRecord.FundingLine.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower())))
            {
                return true;
            }

            int year = Int32.Parse(easRecord.CalendarYear);
            var month = Int32.Parse(easRecord.CalendarMonth);

            var easRecordMonthEndDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            if (_fundingLineContractTypeMappings != null)
            {
                var contractTypesRequired = _fundingLineContractTypeMappings.
                    Where(x => x.FundingLine.Name.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower().Equals(easRecord.FundingLine.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower()))
                    .Select(x => x.ContractType.Name)
                    .Distinct().ToList();

                // bug 98829 : contract re-issue with ended dates that the business want to be "valid" for now
                // return if any contract found
                return _contractAllocations.Any(x => contractTypesRequired.Contains(x.FundingStreamPeriodCode));

                //foreach (var contract in contractAllocations)
                //{
                //    if (contract.EndDate == null || contract.EndDate.GetValueOrDefault() >= easRecordMonthEndDate)
                //    {
                //        return true;
                //    }
                //}

                // bug 98829 end
            }

            return false;
        }

        public bool FundingLineMustHaveValidDates(EasCsvRecord easRecord)
        {
            var contracts =
                _contractAllocations?.Where(x => string.Equals(x.ContractAllocationNumber, x.ContractAllocationNumber,
                    StringComparison.OrdinalIgnoreCase)) ?? Enumerable.Empty<ContractAllocation>();

            if (contracts.Any())
            {
                int year = Int32.Parse(easRecord.CalendarYear);
                var month = Int32.Parse(easRecord.CalendarMonth);
                var date = new DateTime(year, month, 1);

                return contracts.Any(x => x.StartDate <= date && (x.EndDate ?? DateTime.MaxValue) >= date);
            }


            return true;
        }


        private bool DevolvedAreaSourceOfFundingMustHaveAValidDevolvedContract(EasCsvRecord easCsvRecord)
        {
            if (!string.IsNullOrEmpty(easCsvRecord.DevolvedAreaSourceOfFunding))
            {
                bool canParse = int.TryParse(easCsvRecord.DevolvedAreaSourceOfFunding, out var result);
                if (!canParse)
                {
                    return false;
                }

                if (_mcaShortCodeDictionary.TryGetValue(result, out var mcaShortCode))
                {
                    int monthResult;
                    int yearResult;

                    bool monthParse = int.TryParse(easCsvRecord.CalendarMonth, out monthResult);
                    bool yearParse = int.TryParse(easCsvRecord.CalendarYear, out yearResult);

                    if (monthParse && yearParse)
                    {
                        return _devolvedContracts.TryGetValue(mcaShortCode, out var contracts) ? contracts.Any(x => ValidDevolvedContract(x, monthResult, yearResult)) : false;
                    }
                }
            }

            return true;
        }

        private bool DevolvedAreaSourceOfFundingMustBeAValidLookUp(string devolvedSourceOfFunding)
        {
            if (!string.IsNullOrEmpty(devolvedSourceOfFunding))
            {
                bool canParse = int.TryParse(devolvedSourceOfFunding, out var result);
                if (!canParse)
                {
                    return false;
                }

                if (!DataServiceConstants.ValidDevolvedSourceOfFundingCodes.Contains(result))
                {
                    return false;
                }
            }

            return true;
        }

        private bool DevolvedAreaSourceOfFundingMustExist(EasCsvRecord easRecord)
        {
            if (!string.IsNullOrEmpty(easRecord.FundingLine) && 
                _fundingLineContractTypesNotRequired.Any(x => x.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower()
                .Equals(easRecord.FundingLine.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower())))
            {
                if (string.IsNullOrEmpty(easRecord.DevolvedAreaSourceOfFunding))
                {
                    return false;
                }
            }

            return true;
        }

        private bool DevolvedAreaSourceOfFundingMustNotExist(EasCsvRecord easRecord)
        {
            if (!string.IsNullOrEmpty(easRecord.FundingLine) &&
                !_fundingLineContractTypesNotRequired.Any(x => x.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower()
                .Equals(easRecord.FundingLine.RemoveWhiteSpacesNonAlphaNumericCharacters().ToLower())))
            {
                if (!string.IsNullOrEmpty(easRecord.DevolvedAreaSourceOfFunding))
                {
                    return false;
                }
            }

            return true;
        }

        private bool BeAValidYear(string calendarYear)
        {
            var validYears = new List<int> { 2020, 2021 };

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
            var inValidMonthsIn2020 = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            var inValidMonthsIn2021 = new List<int> { 8, 9, 10, 11, 12 };
            if ((Convert.ToInt32(record.CalendarYear) == 2020 && inValidMonthsIn2020.Contains(Convert.ToInt32(record.CalendarMonth)))
                ||
                (Convert.ToInt32(record.CalendarYear) == 2021 && inValidMonthsIn2021.Contains(Convert.ToInt32(record.CalendarMonth))))
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

        private bool ValidDevolvedContract(DevolvedContract devolvedContract, int month, int year)
        {
            var date = new DateTime(year, month, 1);
            return (devolvedContract.EffectiveFrom <= date ||
                (devolvedContract.EffectiveFrom.Year <= year &&
                devolvedContract.EffectiveFrom.Month <= month)) && (devolvedContract.EffectiveTo ?? DateTime.MaxValue) >= date;
        }
    }
}
