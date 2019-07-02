//using ESFA.DC.EAS1819.Stateless.Config;
//using ESFA.DC.ServiceFabric.Helpers.Interfaces;

//namespace ESFA.DC.EAS1819.Stateless.Test
//{
//    public sealed class TestConfigurationHelper : IConfigurationHelper
//    {
//        public T GetSectionValues<T>(string sectionName)
//        {
//            switch (sectionName)
//            {
//                case "EasServiceConfiguration":
//                    return (T)(object)new EasServiceConfiguration
//                    {
//                        EasdbConnectionString = "Server=.;Database=myDataBase;User Id=myUsername;Password = myPassword;",
//                        LoggerConnectionString =
//                            "Server=.;Database=myDataBase;User Id=myUsername;Password = myPassword;",
//                        AuditQueueName = "",
//                        JobStatusQueueName = "",
//                        ServiceBusConnectionString = "",
//                        SubscriptionName = "",
//                        TopicName = ""
//                    };
//            }

//            return default(T);
//        }
//    }
//}
