using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS2021.EF;

namespace ESFA.DC.EAS.DataService.Interface
{
    public interface IValidationErrorRuleService
    {
        Task<List<ValidationErrorRule>> GetAllValidationErrorRules(CancellationToken cancellationToken);
    }
}
