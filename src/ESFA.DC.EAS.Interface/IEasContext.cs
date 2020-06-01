using System;
using System.Collections.Generic;

namespace ESFA.DC.EAS.Interface
{
    public interface IEasJobContext
    {
        long JobId { get; }
    
        string FileReference { get; set; }

        string Container { get; }

        IEnumerable<string> Tasks { get; }

        int Ukprn { get; set; }

        DateTime SubmissionDateTimeUtc { get; }

        int ReturnPeriod { get; set; }

        string ReportOutputFileNames { get; set; }
    }
}
