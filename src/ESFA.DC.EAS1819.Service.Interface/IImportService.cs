using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS1819.Model;

namespace ESFA.DC.EAS1819.Service.Interface
{
   public interface IImportService
   {
       Task ImportEasDataAsync(EasFileInfo fileInfo, IList<EasCsvRecord> easCsvRecords, CancellationToken cancellationToken);
   }
}
