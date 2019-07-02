using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.EF;

namespace ESFA.DC.EAS1819.DataService.Interface
{
    public interface IValidationErrorRuleService
    {
        Task<List<ValidationErrorRule>> GetAllValidationErrorRules(CancellationToken cancellationToken);
    }
}
