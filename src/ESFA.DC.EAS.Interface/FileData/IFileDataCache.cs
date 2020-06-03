using System.Collections.Generic;
using ESFA.DC.EAS.Model;

namespace ESFA.DC.EAS.Interface.FileData
{
    public interface IFileDataCache
    {
        IEnumerable<ValidationErrorModel> ValidationErrors { get; set; }

        IEnumerable<EasCsvRecord> AllEasCsvRecords { get; set; }

        IEnumerable<EasCsvRecord> ValidEasCsvRecords { get; set; }

        string UkPrn { get; set; }

        string Filename { get; set; }

        bool FailedFileValidation { get; set; }
    }
}
