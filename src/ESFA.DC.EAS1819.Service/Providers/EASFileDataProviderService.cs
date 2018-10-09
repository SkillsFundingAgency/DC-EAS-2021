using ESFA.DC.EAS1819.Model;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.EAS1819.Service.Providers
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using ESFA.DC.EAS1819.Service.Interface;

    public class EASFileDataProviderService : IEASDataProviderService
    {
        private readonly IJobContextMessage _jobContextMessage;

        public EASFileDataProviderService()
        {
        }

        public Task<StreamReader> Provide(EasFileInfo easFileInfo, CancellationToken cancellationToken)
        {
            StreamReader streamReader;

           Task<StreamReader> task = Task.Run(
               () =>
               {
                   if (!string.IsNullOrEmpty(easFileInfo.FilePath))
                   {
                       streamReader = File.OpenText(easFileInfo.FilePath);
                   }
                   else
                   {
                       streamReader = File.OpenText(easFileInfo.FileName);
                   }

                   //streamReader = File.OpenText(_filePath);
                   return streamReader;
               },
                cancellationToken: cancellationToken);

            return task;
        }
    }
}
