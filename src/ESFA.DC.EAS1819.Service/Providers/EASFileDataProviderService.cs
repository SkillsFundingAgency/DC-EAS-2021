using System.Linq;

namespace ESFA.DC.EAS1819.Service.Providers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using ESFA.DC.EAS1819.Model;
    using ESFA.DC.EAS1819.Service.Interface;
    using ESFA.DC.EAS1819.Service.Mapper;

    public class EASFileDataProviderService : IEASDataProviderService
    {
        private readonly string _filePath;
        private readonly IFileValidationService _fileValidationService;
        private readonly CancellationToken _cancellationToken;

        public EASFileDataProviderService()
        {
        }

        public EASFileDataProviderService(
                                string filePath,
                                CancellationToken cancellationToken)
        {
            _filePath = filePath;
            _cancellationToken = cancellationToken;
        }

        //public Task<IList<EasCsvRecord>> ProvideData()
        //{
        //    IList<EasCsvRecord> records = null;

        //    Task<IList<EasCsvRecord>> task = Task.Run(
        //        () =>
        //        {
        //            using (StreamReader streamReader = File.OpenText(_filePath))
        //            {
        //                var headers = _csvParser.GetHeaders(streamReader);
        //                var validationErrorModel = _validationService.ValidateHeader(headers);
        //                if (validationErrorModel != null)
        //                {
        //                    throw new InvalidDataException("Invalid Headers");
        //                }
        //                streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
        //                var easCsvRecords = _csvParser.GetData(streamReader, new EasCsvRecordMapper());
        //                var validationResults = _validationService.ValidateData(easCsvRecords.ToList());

        //                return easCsvRecords;
        //            }
        //        },
        //        cancellationToken: _cancellationToken);

        //    return task;
        //}

        public Task<StreamReader> Provide()
        {
            StreamReader streamReader;

           Task<StreamReader> task = Task.Run(
               () =>
               {
                   streamReader = File.OpenText(@"C:\ESFA\DCT\EAS\" + _filePath);
                   return streamReader;
               },
                cancellationToken: _cancellationToken);

            return task;
        }
    }
}
