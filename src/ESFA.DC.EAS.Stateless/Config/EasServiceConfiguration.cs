using ESFA.DC.EAS.Stateless.Config.Interfaces;

namespace ESFA.DC.EAS.Stateless.Config
{
    public class EasServiceConfiguration : IEasServiceConfiguration
    {
        public string ServiceBusConnectionString { get; set; }

        public string MaxMessageSize { get; set; }

        public string TopicName { get; set; }

        public string SubscriptionName { get; set; }

        public string JobStatusQueueName { get; set; }

        public string AuditQueueName { get; set; }

        public string LoggerConnectionString { get; set; }

        public string EasdbConnectionString { get; set; }

        public string AzureBlobConnectionString { get; set; }

        public string AzureBlobContainerName { get; set; }
    }
}
