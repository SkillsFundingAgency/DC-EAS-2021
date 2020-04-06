using System.Collections.Generic;
using ESFA.DC.ReferenceData.FCS.Model;

namespace ESFA.DC.EAS.DataService.Interface.FCS
{
    public interface IFCSDataService
    {
        List<ContractAllocation> GetContractsForProvider(int ukprn);
    }
}
