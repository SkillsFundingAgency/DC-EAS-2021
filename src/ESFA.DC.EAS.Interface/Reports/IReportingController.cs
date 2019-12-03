using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.EAS.Interface.Reports
{
    public interface IReportingController
    {
        Task ProduceReportsAsync(
            IJobContextMessage jobContextMessage,
            IList<EasCsvRecord> models,
            IList<ValidationErrorModel> errors,
            EasFileInfo sourceFile,
            CancellationToken cancellationToken);

        Task FileLevelErrorReportAsync(
            IList<EasCsvRecord> models,
            EasFileInfo fileInfo,
            IList<ValidationErrorModel> errors,
            CancellationToken cancellationToken);
    }
}
