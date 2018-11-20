using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Model;

namespace ESFA.DC.EAS1819.Tests.Base.Builders
{
    public class ValidationErrorModelBuilder
    {
        public static implicit operator List<ValidationErrorModel>(ValidationErrorModelBuilder instance)
        {
            return instance.Build();
        }

        public List<ValidationErrorModel> Build()
        {
            return GetValidationErrorModels();
        }

        public List<ValidationErrorModel> GetValidationErrorModels()
        {
            var errors = new List<ValidationErrorModel>()
            {
                new ValidationErrorModel()
                {
                    RuleName = "CalendarMonth_01",
                    ErrorMessage = "The Calendar Month is not valid.",
                    Severity = "E",
                    FundingLine = "16-18 Apprenticeships",
                    AdjustmentType = "Excess Learning Support",
                    CalendarYear = 2018,
                    CalendarMonth = 900,
                    Value = "10000003.68"
                },
                new ValidationErrorModel()
                {
                    RuleName = "Value_01",
                    ErrorMessage = "The value field must be entered.",
                    Severity = "E",
                    FundingLine = "24+ Apprenticeships",
                    AdjustmentType = "Authorised Claims",
                    CalendarYear = 2018,
                    CalendarMonth = 8
                }
            };
            return errors;
        }
    }
}
