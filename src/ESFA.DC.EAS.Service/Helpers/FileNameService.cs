using System;
using System.Collections.Generic;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS.Interface;

namespace ESFA.DC.EAS.Service.Helpers
{
    public class FileNameService : IFileNameService
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly IDictionary<OutputTypes, string> _extensionsDictionary = new Dictionary<OutputTypes, string>()
        {
            [OutputTypes.Csv] = "csv",
            [OutputTypes.Excel] = "xlsx",
            [OutputTypes.Json] = "json",
            [OutputTypes.Zip] = "zip",
        };

        public FileNameService(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public string GetFilename(string ukPrn, long jobId, string fileName, DateTime submissionDateTime, OutputTypes outputType)
        {
            DateTime dateTime = _dateTimeProvider.ConvertUtcToUk(submissionDateTime);
            return $"{ukPrn}/{jobId}/{fileName}-{dateTime:yyyyMMdd-HHmmss}.{GetExtension(outputType)}";
        }

        public string GetZipName(string ukPrn, long jobId, string zipName)
        {
            return $"{ukPrn}/{jobId}/{zipName}.{GetExtension(OutputTypes.Zip)}";
        }

        private string GetExtension(OutputTypes outputType) => _extensionsDictionary[outputType];
    }
}
