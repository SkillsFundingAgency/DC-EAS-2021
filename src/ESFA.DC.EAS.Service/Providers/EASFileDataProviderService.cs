using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.EAS.Model;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.EAS.Service.Providers
{
    public class EASFileDataProviderService : IEASDataProviderService
    {
        public async Task<IList<EasCsvRecord>> ProvideData(IJobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<StreamReader> ProvideAsync(EasFileInfo easFileInfo, CancellationToken cancellationToken)
        {
            StreamReader streamReader;

            if (!string.IsNullOrEmpty(easFileInfo.FilePath))
            {
                streamReader = File.OpenText(easFileInfo.FilePath);
            }
            else
            {
                streamReader = File.OpenText(easFileInfo.FileName);
            }

            return streamReader;
        }
    }
}
