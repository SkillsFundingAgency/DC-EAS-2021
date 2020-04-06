using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.EAS.DataService.Interface.FCS;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;

namespace ESFA.DC.EAS.DataService.FCS
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
            var contractAllocations = _fcsContext.ContractAllocations.Where(x => x.DeliveryUkprn == Ukprn).ToList();
            return contractAllocations;
        }
    }
}
