using System;

namespace ESFA.DC.EAS.Interface
{
    public interface IFileNameService
    {
        string GetFilename(string ukPrn, long jobId, string fileName, DateTime submissionDateTime, OutputTypes outputType);

        string GetZipName(string ukPrn, long jobId, string zipName);
    }
}
