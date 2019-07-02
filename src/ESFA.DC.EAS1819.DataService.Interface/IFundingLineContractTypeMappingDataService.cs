using System.Threading.Tasks;

namespace ESFA.DC.EAS1819.DataService.Interface
{
    using System.Collections.Generic;
    using System.Threading;
    using EF;

    public interface IFundingLineContractTypeMappingDataService
    {
        Task<List<FundingLineContractTypeMapping>> GetAllFundingLineContractTypeMappings(CancellationToken cancellationToken);
    }
}
