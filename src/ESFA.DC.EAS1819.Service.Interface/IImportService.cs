using System.IO;
using ESFA.DC.EAS1819.Model;

namespace ESFA.DC.EAS1819.Service.Interface
{
   public interface IImportService
   {
       void ImportEasData(EasFileInfo fileInfo);
   }
}
