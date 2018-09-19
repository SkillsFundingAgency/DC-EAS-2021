namespace ESFA.DC.EAS1819.Service.Providers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CsvHelper;
    using ESFA.DC.EAS1819.Model;
    using ESFA.DC.EAS1819.Service.Interface;
    using ESFA.DC.EAS1819.Service.Mapper;
    using ESFA.DC.JobContext.Interface;

    public class EASFileDataProviderService : IEASDataProviderService
    {
        private readonly string _filePath;
        private readonly CancellationToken _cancellationToken;

        public EASFileDataProviderService(string filePath, CancellationToken cancellationToken)
        {
            _filePath = filePath;
            _cancellationToken = cancellationToken;
        }

        public Task<IList<EasCsvRecord>> Provide()
        {
            IList<EasCsvRecord> records = null;

            Task<IList<EasCsvRecord>> task = Task.Run(
                () =>
                {
                    using (TextReader fileReader = File.OpenText(_filePath))
                    {
                        var csv = new CsvReader(fileReader);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.RegisterClassMap<EasCsvRecordMapper>();
                        records = csv.GetRecords<EasCsvRecord>().ToList();
                        return records;
                    }
                },
                cancellationToken: _cancellationToken);

            return task;
        }
    }
}
