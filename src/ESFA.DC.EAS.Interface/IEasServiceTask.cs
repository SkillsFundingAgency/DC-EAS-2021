using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.EAS.Interface
{
    public interface IEasServiceTask
    {
        string TaskName { get; }

        Task ExecuteAsync(IJobContextMessage jobContextMessage, EasFileInfo easFileInfo, CancellationToken cancellationToken);
    }
}
