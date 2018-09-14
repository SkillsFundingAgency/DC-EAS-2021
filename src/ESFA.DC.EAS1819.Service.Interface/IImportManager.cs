using System.IO;

namespace ESFA.DC.EAS1819.Service.Interface
{
   public interface IImportManager
   {
       void ImportEasCsv(TextReader reader);
   }
}
