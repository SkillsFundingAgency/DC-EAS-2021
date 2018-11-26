namespace ESFA.DC.EAS1819.DataService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ESFA.DC.EAS1819.DataService.Interface;
    using ESFA.DC.EAS1819.EF;

    public class ValidationErrorRuleService : IValidationErrorRuleService
    {
        private readonly IRepository<ValidationErrorRule> _validationErrorRuleRepository;

        public ValidationErrorRuleService(IRepository<ValidationErrorRule> validationErrorRuleRepository)
        {
            _validationErrorRuleRepository = validationErrorRuleRepository;
        }

        public List<ValidationErrorRule> GetAllValidationErrorRules()
        {
            var query = _validationErrorRuleRepository.TableNoTracking.OrderBy(s => s.RuleId);
            var validationErrorRules = query.ToList();
            return validationErrorRules;
        }
    }
}