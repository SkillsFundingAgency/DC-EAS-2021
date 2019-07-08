using System.Collections.Generic;
using System.IO;
using CsvHelper.Configuration;

namespace ESFA.DC.EAS.Interface
{
    public interface ICsvParser
    {
        IList<T> GetData<T>(StreamReader reader, ClassMap<T> mapper)
            where T : class;

        IList<string> GetHeaders(StreamReader reader);
    }
}
