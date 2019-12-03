using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Model;

namespace ESFA.DC.EAS.Interface.Reports
{
    public interface IModelReport
    {
        Task<IEnumerable<string>> GenerateReportAsync(
            IList<EasCsvRecord> data,
            EasFileInfo fileInfo,
            IList<ValidationErrorModel> validationErrors,
            ZipArchive archive,
            CancellationToken cancellationToken);
    }
}
