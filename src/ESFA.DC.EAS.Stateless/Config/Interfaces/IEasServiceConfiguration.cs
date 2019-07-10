namespace ESFA.DC.EAS.Stateless.Config.Interfaces
{
    public interface IEasServiceConfiguration
    {
        string AuditQueueName { get; set; }

        string AzureBlobConnectionString { get; set; }

        string AzureBlobContainerName { get; set; }

        string EasdbConnectionString { get; set; }

        string JobStatusQueueName { get; set; }

        string LoggerConnectionString { get; set; }

        string ServiceBusConnectionString { get; set; }

        string SubscriptionName { get; set; }

        string TopicName { get; set; }
    }
}