using System.Collections.Generic;
using System.Linq;
using ESFA.DC.EAS1819.DataService.Interface;
using ESFA.DC.EAS1819.EF;

namespace ESFA.DC.EAS1819.DataService
{
   public class FundingLineContractTypeMappingDataService : IFundingLineContractTypeMappingDataService
    {
        private readonly IRepository<FundingLineContractTypeMapping> _repository;

        public FundingLineContractTypeMappingDataService(IRepository<FundingLineContractTypeMapping> repository)
        {
            _repository = repository;
        }

        public List<FundingLineContractTypeMapping> GetAllFundingLineContractTypeMappings()
        {
            var fundingLineContractTypeMappings = _repository.AllIncluding(x => x.FundingLine, y => y.ContractType).ToList();
            return fundingLineContractTypeMappings;
        }
    }
}
