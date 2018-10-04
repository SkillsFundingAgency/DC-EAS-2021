using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using Xunit;

namespace ESFA.DC.EAS1819.Stateless.Test
{

    public sealed class AutoFacTest
    {
        [Fact]
        public void TestRegistrations()
        {
            JobContextMessage jobContextMessage =
                new JobContextMessage(
                    1,
                    new ITopicItem[] { new TopicItem("SubscriptionName", new List<ITaskItem>()) },
                    0,
                    DateTime.UtcNow);

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Cancel();

            ContainerBuilder containerBuilder = DIComposition.BuildContainer(new TestConfigurationHelper());
            IContainer c;
            try
            {
                c = containerBuilder.Build();

                using (var lifeTime = c.BeginLifetimeScope())
                {
                    var messageHandler = lifeTime.Resolve<IJobContextManager<JobContextMessage>>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
