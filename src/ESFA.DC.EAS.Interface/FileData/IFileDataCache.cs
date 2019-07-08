using System;

namespace ESFA.DC.EAS.Interface.FileData
{
    using System.Collections.Generic;
    using ESFA.DC.EAS.Model;

    public interface IFileDataCache
    {
        List<ValidationErrorModel> ValidationErrors { get; set; }

        List<EasCsvRecord> AllEasCsvRecords { get; set; }

        List<EasCsvRecord> ValidEasCsvRecords { get; set; }

        string UkPrn { get; set; }

        string Filename { get; set; }

        bool FailedFileValidation { get; set; }
    }
}
