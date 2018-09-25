using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace ESFA.DC.EAS1819.Service.Validation
{
   public class TestValidatorFactory : IValidatorFactory
    {
        public IValidator<T> GetValidator<T>()
        {
            return (IValidator<T>)this.GetValidator(typeof(T));
        }

        public IValidator GetValidator(Type type)
        {
            if (type == typeof(StreamReader))
            {
                return new FileValidator();
            }

            return null;
        }
    }
}
