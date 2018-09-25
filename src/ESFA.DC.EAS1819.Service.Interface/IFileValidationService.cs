using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ESFA.DC.EAS1819.Service.Interface
{
    public interface IFileValidationService
    {
       bool IsValid(StreamReader reader);
    }
}
