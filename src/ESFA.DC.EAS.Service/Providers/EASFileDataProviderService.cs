using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CsvService.Interface;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Model;
using ESFA.DC.EAS.Service.Exceptions;
using ESFA.DC.EAS.Service.Mapper;

namespace ESFA.DC.EAS.Service.Providers
{
    public class EASFileDataProviderService : IEASFileDataProviderService
    {
        private readonly ICsvFileService _csvFileService;

        public EASFileDataProviderService(ICsvFileService csvFileService)
        {
            _csvFileService = csvFileService;
        }

        public async Task<List<EasCsvRecord>> ProvideData(string fileName, string container, CancellationToken cancellationToken)
        {
            ICollection<EasCsvRecord> models = new List<EasCsvRecord>();

            try
            {
                models = await _csvFileService.ReadAllAsync<EasCsvRecord, EasCsvRecordMapper>(fileName, container, cancellationToken);
            }
            catch (System.Exception e)
            {
                throw new InvalidCsvException("Error parsing csv file. ", e);
            }

            return models.ToList();
        }
    }
}
