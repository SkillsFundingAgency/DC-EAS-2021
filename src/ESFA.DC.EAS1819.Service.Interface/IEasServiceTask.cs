using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.EAS1819.Service.Interface
{
    public interface IEasServiceTask
    {
        string TaskName { get; }

        Task ExecuteAsync(IJobContextMessage jobContextMessage, EasFileInfo easFileInfo, IList<EasCsvRecord> easCsvRecords, CancellationToken cancellationToken);
    }
}
