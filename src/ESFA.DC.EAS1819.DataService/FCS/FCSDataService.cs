using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.DataService.Interface.FCS;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;

namespace ESFA.DC.EAS1819.DataService.FCS
{
   public class FCSDataService : IFCSDataService
    {
        private readonly IFcsContext _fcsContext;

        public FCSDataService(IFcsContext fcsContext)
        {
            _fcsContext = fcsContext;
        }

        public List<ContractAllocation> GetContractsForProvider(int Ukprn)
        {
            var contractAllocations = _fcsContext.ContractAllocations.Where(x => x.Contract.Contractor.Ukprn == 10000421)
                                            .Select(x => new ContractAllocation
                                            {
                                                FundingStreamCode = x.FundingStreamCode,
                                                StartDate = x.StartDate,
                                                EndDate = x.EndDate
                                            }).ToList();
            return contractAllocations;
        }
    }
}
