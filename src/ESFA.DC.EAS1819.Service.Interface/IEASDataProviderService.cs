using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.EAS1819.Service.Interface
{
    public interface IEASDataProviderService
    {
        Task<IList<EasCsvRecord>> Provide();
    }
}
