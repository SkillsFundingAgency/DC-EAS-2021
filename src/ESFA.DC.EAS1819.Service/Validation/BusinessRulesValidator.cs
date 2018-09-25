using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.Model;
using FluentValidation;

namespace ESFA.DC.EAS1819.Service.Validation
{
    public class BusinessRulesValidator : AbstractValidator<EasCsvRecord>
    {
        private readonly IEasPaymentService _paymentService;

        public BusinessRulesValidator()
        {
          RuleFor(x => x.CalendarMonth)
                .InclusiveBetween(1, 12)
                .WithMessage("The Calendar Month is not valid")
                .WithErrorCode("CalendarMonth_01");

            RuleFor(x => x.CalendarYear)
                .InclusiveBetween(2018, 2019)
                .WithMessage("The CalendarYear is not valid")
                .WithErrorCode("CalendarYear_01");
        }
    }
}
