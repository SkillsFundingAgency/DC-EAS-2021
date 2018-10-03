using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;

namespace ESFA.DC.EAS1819.Stateless
{
    public class JobContextMessageHandler :  IMessageHandler<JobContextMessage>
    {
        public JobContextMessageHandler()
        {
            
        }
        public Task<bool> HandleAsync(JobContextMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
