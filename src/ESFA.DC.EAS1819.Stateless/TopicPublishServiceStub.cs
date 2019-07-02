using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Queueing.Interface;

namespace ESFA.DC.EAS1819.Stateless
{
    public class TopicPublishServiceStub<T> : ITopicPublishService<T>
        where T : new()
    {
        public Task PublishAsync(T obj, IDictionary<string, object> properties, string messageLabel)
        {
            throw new NotImplementedException();
        }
    }
}
