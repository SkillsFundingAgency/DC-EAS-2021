﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.FileService.Interface.Model;

namespace ESFA.DC.EAS.Acceptance.Test.Stubs
{
    public class FileServiceStub : IFileService
    {
        public async Task<Stream> OpenReadStreamAsync(string fileReference, string container, CancellationToken cancellationToken)
        {
            return await Task.FromResult(File.OpenRead(Path.Combine(container, fileReference)) as Stream);
        }

        public async Task<Stream> OpenWriteStreamAsync(string fileReference, string container, CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(container);

            return await Task.FromResult(File.OpenWrite(Path.Combine(container, fileReference)) as Stream);
        }

        public Task<IEnumerable<string>> GetFileReferencesAsync(string container, string prefix, bool includeSubfolders,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetFileReferencesAsync(string container, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(string fileReference, string container, CancellationToken cancellationToken)
        {
            return Task.FromResult(File.Exists(Path.Combine(container, fileReference)));
        }

        public Task<IEnumerable<string>> GetFileReferencesAsync(string container, string prefix, bool includeSubfolders, CancellationToken cancellationToken, bool includeDeletedBlobs = true)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetFileReferencesAsync(string container, CancellationToken cancellationToken, bool includeDeletedBlobs = true)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileMetaData>> GetFileMetaDataAsync(string container, string prefix, bool includeSubfolders, CancellationToken cancellationToken, bool includeDeletedBlobs = true)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileMetaData>> GetFileMetaDataAsync(string container, CancellationToken cancellationToken, bool includeDeletedBlobs = true)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFolderAsync(string folderReference, string container, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileAsync(string fileReference, string container, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
