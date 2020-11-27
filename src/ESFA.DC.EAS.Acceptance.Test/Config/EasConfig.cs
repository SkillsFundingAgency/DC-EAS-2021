using ESFA.DC.EAS.Interface.Config;

namespace ESFA.DC.EAS.Acceptance.Test
{
    public partial class EasAcceptanceTests
    {
        public class EasConfig : IEasServiceConfiguration
        {
            public string LoggerConnectionString { get; set; }
            public string AuditQueueName { get; set; }
            public string AzureBlobConnectionString { get; set; }
            public string AzureBlobContainerName { get; set; }
            public string EasdbConnectionString { get; set; }
            public string JobStatusQueueName { get; set; }
            public string ServiceBusConnectionString { get; set; }
            public string SubscriptionName { get; set; }
            public string TopicName { get; set; }
        }
    }
}
