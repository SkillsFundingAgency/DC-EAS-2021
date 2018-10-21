using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Model;

namespace ESFA.DC.EAS1819.Interface
{
    public interface IEASDataProviderService
    {
        Task<StreamReader> ProvideAsync(EasFileInfo easFileInfo, CancellationToken cancellationToken);
    }
}
