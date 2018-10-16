using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ReferenceData.FCS.Model;

namespace ESFA.DC.EAS1819.DataService.Interface.FCS
{
    public interface IFCSDataService
    {
        List<ContractAllocation> GetContractsForProvider(int Ukprn);
    }
}
