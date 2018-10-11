using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.EAS1819.DataService.Interface
{
    public interface IFundingLineContractTypeMappingDataService
    {
       List<string> GetContractTypesRequired(string fundingLine);
    }
}
