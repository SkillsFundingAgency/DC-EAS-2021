using ESFA.DC.IO.AzureStorage.Config.Interfaces;

namespace ESFA.DC.EAS1819.Acceptance.Test
{
    public partial class EasAcceptanceTests
    {
        public class AzureStorageConfig : IAzureStorageKeyValuePersistenceServiceConfig
        {
            public AzureStorageConfig(string connectionString, string containerName)
            {
                AzureBlobConnectionString = connectionString;
                AzureContainerReference = containerName;
            }

            public string AzureBlobConnectionString { get; set; }

            public string AzureContainerReference { get; set; }

            public string ConnectionString => AzureBlobConnectionString;

            public string ContainerName => AzureContainerReference;
        }
    }
}
