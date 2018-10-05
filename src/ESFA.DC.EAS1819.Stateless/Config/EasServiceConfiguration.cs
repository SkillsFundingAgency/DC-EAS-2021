namespace ESFA.DC.EAS1819.Stateless.Config
{
    public class EasServiceConfiguration
    {
        public string ServiceBusConnectionString { get; set; }

        public string TopicName { get; set; }

        public string SubscriptionName { get; set; }

        public string JobStatusQueueName { get; set; }

        public string AuditQueueName { get; set; }

        public string LoggerConnectionString { get; set; }

        public string EasdbConnectionString { get; set; }
    }
}
