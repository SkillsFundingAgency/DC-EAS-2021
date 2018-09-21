using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.EF;

namespace ESFA.DC.EAS1819.DataService
{
    public class ValidationErrorService
    {
        private readonly IRepository<ValidationError> _validationErroRepository;

        public ValidationErrorService(IRepository<ValidationError> validationErroRepository)
        {
            _validationErroRepository = validationErroRepository;
        }

        public void LogValidationError(ValidationError validationError)
        {
            _validationErroRepository.Insert(validationError);
        }
    }
}
