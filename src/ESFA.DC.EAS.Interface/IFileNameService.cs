using System;

namespace ESFA.DC.EAS.Interface
{
    public interface IFileNameService
    {
        string GetFilename(int ukPrn, long jobId, string fileName, DateTime submissionDateTime, OutputTypes outputType);

        string GetZipName(int ukPrn, long jobId, string zipName);
    }
}
