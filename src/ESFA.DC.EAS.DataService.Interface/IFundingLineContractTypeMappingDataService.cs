using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1920.EF;

namespace ESFA.DC.EAS.DataService.Interface
{
    public interface IFundingLineContractTypeMappingDataService
    {
        Task<List<FundingLineContractTypeMapping>> GetAllFundingLineContractTypeMappings(CancellationToken cancellationToken);
    }
}
