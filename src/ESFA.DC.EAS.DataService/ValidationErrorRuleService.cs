using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS1920.EF;
using ESFA.DC.EAS1920.EF.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.EAS.DataService
{
    public sealed class ValidationErrorRuleService : IValidationErrorRuleService
    {
        private readonly IEasdbContext _repository;

        public ValidationErrorRuleService(IEasdbContext repository)
        {
            _repository = repository;
        }

        public async Task<List<ValidationErrorRule>> GetAllValidationErrorRules(CancellationToken cancellationToken)
        {
            List<ValidationErrorRule> validationErrorRules = await _repository.ValidationErrorRules.OrderBy(s => s.RuleId).ToListAsync(cancellationToken);
            return validationErrorRules;
        }
    }
}