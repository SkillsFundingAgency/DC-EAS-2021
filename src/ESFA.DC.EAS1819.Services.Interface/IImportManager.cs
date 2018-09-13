using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ESFA.DC.EAS1819.Services.Interface
{
   public interface IImportManager
   {
       void ImportEasCsv(TextReader reader);
   }
}
