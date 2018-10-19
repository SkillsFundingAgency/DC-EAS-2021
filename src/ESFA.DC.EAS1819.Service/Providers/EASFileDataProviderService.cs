namespace ESFA.DC.EAS1819.Service.Providers
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using ESFA.DC.EAS1819.Model;
    using ESFA.DC.EAS1819.Service.Interface;
    using ESFA.DC.JobContextManager.Model.Interface;

    public class EASFileDataProviderService : IEASDataProviderService
    {
        private readonly IJobContextMessage _jobContextMessage;

        public EASFileDataProviderService()
        {
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
