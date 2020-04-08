using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.DataService.Interface;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.EAS2021.EF.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.EAS.DataService
{
   public class FundingLineContractTypeMappingDataService : IFundingLineContractTypeMappingDataService
    {
        private IEasdbContext _repository;

        public FundingLineContractTypeMappingDataService(IEasdbContext repository)
        {
            _repository = repository;
        }

        public async Task<List<FundingLineContractTypeMapping>> GetAllFundingLineContractTypeMappings(CancellationToken cancellationToken)
        {
            var fundingLineContractTypeMappings = await _repository.FundingLineContractTypeMappings.Include(x => x.FundingLine).Include(x => x.ContractType).ToListAsync(cancellationToken);
            return fundingLineContractTypeMappings;
        }
    }
}
