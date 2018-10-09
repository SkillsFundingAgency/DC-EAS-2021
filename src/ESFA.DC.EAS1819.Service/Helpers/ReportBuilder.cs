using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Service.Interface;

namespace ESFA.DC.EAS1819.Service.Helpers
{
    public static class ReportBuilder
    {
        /// <summary>
        /// Builds a CSV report using the specified mapper as the list of column names.
        /// </summary>
        /// <typeparam name="TMapper">The mapper.</typeparam>
        /// <typeparam name="TModel">The model.</typeparam>
        /// <param name="writer">The memory stream to write to.</param>
        /// <param name="records">The records to persist.</param>
        public static void BuildCsvReport<TMapper, TModel>(MemoryStream writer, IEnumerable<TModel> records)
            where TMapper : ClassMap, IClassMapper
            where TModel : class
        {
            UTF8Encoding utF8Encoding = new UTF8Encoding(false, true);
            using (TextWriter textWriter = new StreamWriter(writer, utF8Encoding, 1024, true))
            {
                using (CsvWriter csvWriter = new CsvWriter(textWriter))
                {
                    csvWriter.Configuration.RegisterClassMap<TMapper>();
                    csvWriter.WriteHeader<TModel>();
                    csvWriter.NextRecord();
                    csvWriter.WriteRecords(records);
                }
            }
        }
    }
}
