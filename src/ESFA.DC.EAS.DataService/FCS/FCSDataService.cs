using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.DataService.Interface.FCS;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.EAS.DataService.FCS
{
    public class FCSDataService : IFCSDataService
    {
        private readonly IFcsContext _fcsContext;

        public FCSDataService(IFcsContext fcsContext)
        {
            _fcsContext = fcsContext;
        }

        public async Task<List<ContractAllocation>> GetContractsForProviderAsync(int Ukprn, CancellationToken cancellationToken)
        {
            var contractAllocations = await _fcsContext.ContractAllocations.Where(x => x.DeliveryUkprn == Ukprn).ToListAsync(cancellationToken);
            return contractAllocations;
        }

        public async Task<IReadOnlyDictionary<string, IEnumerable<DevolvedContract>>> GetDevolvedContractsForProviderAsync(int ukprn, CancellationToken cancellationToken)
        {
            var contracts = await _fcsContext.DevolvedContracts
                    .Where(dc => dc.Ukprn == ukprn)
                    .ToListAsync(cancellationToken);

            var shortCodeDictionary = contracts?
                .GroupBy(x => x.McaglashortCode)
                .ToDictionary(
                k => k.Key,
                v => v.Select(x => x), StringComparer.OrdinalIgnoreCase);

            return shortCodeDictionary;
        }
    }
}
