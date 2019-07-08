using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.Model;

namespace ESFA.DC.EAS.Service.FileData
{
    public class FileDataCache : IFileDataCache
    {
        public List<ValidationErrorModel> ValidationErrors { get; set; }

        public List<EasCsvRecord> AllEasCsvRecords { get; set; }

        public List<EasCsvRecord> ValidEasCsvRecords { get; set; }

        public string UkPrn { get; set; }

        public string Filename { get; set; }

        public bool FailedFileValidation { get; set; }
    }
}
