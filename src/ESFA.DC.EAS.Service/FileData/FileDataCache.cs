using System.Collections.Generic;
using ESFA.DC.EAS.Interface.FileData;
using ESFA.DC.EAS.Model;

namespace ESFA.DC.EAS.Service.FileData
{
    public class FileDataCache : IFileDataCache
    {
        public IEnumerable<ValidationErrorModel> ValidationErrors { get; set; }

        public IEnumerable<EasCsvRecord> AllEasCsvRecords { get; set; }

        public IEnumerable<EasCsvRecord> ValidEasCsvRecords { get; set; }

        public string UkPrn { get; set; }

        public string Filename { get; set; }

        public bool FailedFileValidation { get; set; }
    }
}
