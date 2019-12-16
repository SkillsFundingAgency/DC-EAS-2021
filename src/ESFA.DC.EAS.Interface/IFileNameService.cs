using System;

namespace ESFA.DC.EAS.Interface
{
    public interface IFileNameService
    {
        string GetExternalFilename(string ukPrn, long jobId, string fileName, DateTime submissionDateTime, OutputTypes outputType);

        string GetFilename(string fileName, DateTime submissionDateTime, OutputTypes outputType);
    }
}
