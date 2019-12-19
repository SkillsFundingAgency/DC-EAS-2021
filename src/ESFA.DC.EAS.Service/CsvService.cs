using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using ESFA.DC.EAS.Interface;
using ESFA.DC.FileService.Interface;

namespace ESFA.DC.EAS.Service
{
    public class CsvService : ICsvService
    {
        private readonly UTF8Encoding _encoding = new UTF8Encoding(true, true);

        private readonly IFileService _fileService;

        public CsvService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task WriteAsync<T, TClassMap>(IEnumerable<T> rows, string fileName, string container, CancellationToken cancellationToken)
            where TClassMap : ClassMap<T>
        {
            using (Stream stream = await _fileService.OpenWriteStreamAsync(fileName, container, cancellationToken))
            {
                using (TextWriter textWriter = new StreamWriter(stream, _encoding))
                {
                    var configuration = BuildConfiguration<T, TClassMap>();

                    using (CsvWriter csvWriter = new CsvWriter(textWriter, configuration))
                    {
                        csvWriter.WriteRecords(rows);
                    }
                }
            }
        }

        private static Configuration BuildConfiguration<T, TClassMap>() where TClassMap : ClassMap<T>
        {
            var configuration = new Configuration();

            configuration.RegisterClassMap<TClassMap>();
            configuration.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = new[] { "dd/MM/yyyy" };
            configuration.TypeConverterOptionsCache.GetOptions<DateTime?>().Formats = new[] { "dd/MM/yyyy" };
            return configuration;
        }
    }
}
