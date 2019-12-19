using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS.Interface;
using ESFA.DC.FileService.Interface;

namespace ESFA.DC.EAS.Service
{
    public class ZipService : IZipService
    {
        private readonly IFileService _fileService;

        public ZipService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task CreateZipAsync(string zipName, IEnumerable<string> fileNames, string container, CancellationToken cancellationToken)
        {
            using (var writeStream = await _fileService.OpenWriteStreamAsync(zipName, container, cancellationToken))
            {
                using (var zipArchive = new ZipArchive(writeStream, ZipArchiveMode.Create, true))
                {
                    foreach (var fileName in fileNames.Where(f => !string.IsNullOrWhiteSpace(f)))
                    {
                        var archiveEntry = zipArchive.CreateEntry(Path.GetFileName(fileName));

                        using (var archiveEntryStream = archiveEntry.Open())
                        {
                            using (var readStream = await _fileService.OpenReadStreamAsync(fileName, container, cancellationToken))
                            {
                                await readStream.CopyToAsync(archiveEntryStream, 8096, cancellationToken);
                            }
                        }
                    }
                }
            }
        }
    }
}
