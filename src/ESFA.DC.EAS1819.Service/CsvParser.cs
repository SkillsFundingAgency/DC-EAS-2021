using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using ESFA.DC.EAS1819.Interface;

namespace ESFA.DC.EAS1819.Service
{
    public class CsvParser : ICsvParser
    {
        public IList<T> GetData<T>(StreamReader reader, ClassMap<T> mapper)
                                                where T : class
        {
            var csv = new CsvReader(reader);
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.RegisterClassMap(mapper);
            List<T> records = csv.GetRecords<T>().ToList();
            return records;
        }

        public IList<string> GetHeaders(StreamReader reader)
        {
            var csv = new CsvReader(reader);
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.IgnoreBlankLines = true;
            csv.Read();
            csv.ReadHeader();
            var headerRecords = csv.Context.HeaderRecord.ToList();
            return headerRecords;
        }
    }
}
