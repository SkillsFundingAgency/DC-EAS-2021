using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.EAS1819.Interface;
using ESFA.DC.EAS1819.Model;
using ESFA.DC.EAS1819.Service.Helpers;
using ESFA.DC.EAS1819.Service.Mapper;
using ESFA.DC.IO.Interfaces;

namespace ESFA.DC.EAS1819.ReportingService.Reports
{
    public class ViolationReport : AbstractReportBuilder, IValidationReport
    {
        private readonly IStreamableKeyValuePersistenceService _streamableKeyValuePersistenceService;

        public ViolationReport(
            IDateTimeProvider dateTimeProvider,
            [KeyFilter(PersistenceStorageKeys.AzureStorage)] IStreamableKeyValuePersistenceService streamableKeyValuePersistenceService) : base(dateTimeProvider)
        {
            _streamableKeyValuePersistenceService = streamableKeyValuePersistenceService;
            ReportFileName = "EAS Violation Report";
        }

        public async Task GenerateReport(
            IList<EasCsvRecord> data,
            EasFileInfo fileInfo,
            IList<ValidationErrorModel> validationErrors,
            ZipArchive archive,
            CancellationToken cancellationToken)
        {
            var csv = GetCsv(validationErrors);

            var externalFileName = GetExternalFilename(fileInfo.UKPRN, fileInfo.JobId, fileInfo.DateTime);
            var fileName = GetFilename(fileInfo.UKPRN, fileInfo.JobId, fileInfo.DateTime);

            await _streamableKeyValuePersistenceService.SaveAsync($"{externalFileName}.csv", csv, cancellationToken);
            await WriteZipEntry(archive, $"{fileName}.csv", csv);
        }

        private string GetCsv(IList<ValidationErrorModel> validationErrors)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BuildCsvReport<EasCsvViolationRecordMapper, ValidationErrorModel>(ms, validationErrors);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
