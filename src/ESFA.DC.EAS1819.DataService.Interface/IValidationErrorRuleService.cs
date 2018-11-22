using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.EF;

namespace ESFA.DC.EAS1819.DataService.Interface
{
    public interface IValidationErrorRuleService
    {
        List<ValidationErrorRule> GetAllValidationErrorRules();
    }
}
