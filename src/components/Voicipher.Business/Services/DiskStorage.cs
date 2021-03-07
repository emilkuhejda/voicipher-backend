using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Services
{
    public abstract class DiskStorage : IDiskStorage
    {
        private const string UploadedFilesDirectory = "uploads";

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _filesDirectory;

        protected DiskStorage(IWebHostEnvironment webHostEnvironment, string filesDirectory)
        {
            _webHostEnvironment = webHostEnvironment;
            _filesDirectory = filesDirectory;
        }

        public Task<string> UploadAsync(byte[] bytes, CancellationToken cancellationToken)
        {
            return UploadAsync(bytes, new DiskStorageSettings(), cancellationToken);
        }

        public async Task<string> UploadAsync(byte[] bytes, DiskStorageSettings diskStorageSettings, CancellationToken cancellationToken)
        {
            var fileName = string.IsNullOrWhiteSpace(diskStorageSettings.FileName) ? $"{Guid.NewGuid()}.tmp" : diskStorageSettings.FileName;
            var rootPath = GetDirectoryPath(diskStorageSettings.FolderName ?? string.Empty);
            var filePath = Path.Combine(rootPath, fileName);
            await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);

            return filePath;
        }

        public async Task<byte[]> ReadAllBytesAsync(FileChunk[] fileChunks, CancellationToken cancellationToken)
        {
            var source = new List<byte>();
            foreach (var fileChunk in fileChunks.OrderBy(x => x.Order))
            {
                var bytes = await File.ReadAllBytesAsync(fileChunk.Path, cancellationToken);
                source.AddRange(bytes);
            }

            return source.ToArray();
        }

        public void Delete(DiskStorageSettings diskStorageSettings)
        {
            var rootPath = GetDirectoryPath(diskStorageSettings.FolderName);
            var filePath = Path.Combine(rootPath, diskStorageSettings.FileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public void DeleteRange(FileChunk[] fileChunks)
        {
            foreach (var fileChunk in fileChunks)
            {
                if (File.Exists(fileChunk.Path))
                {
                    File.Delete(fileChunk.Path);
                }
            }
        }

        public void DeleteFolder()
        {
            DeleteFolder(string.Empty);
        }

        public void DeleteFolder(string folderName)
        {
            var rootPath = GetDirectoryPath(folderName);
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, true);
            }
        }

        public string GetDirectoryPath()
        {
            return GetDirectoryPath(string.Empty);
        }

        public string GetDirectoryPath(string folderName)
        {
            var rootDirectoryPath = Path.Combine(_webHostEnvironment.WebRootPath, UploadedFilesDirectory, _filesDirectory, folderName);
            if (!Directory.Exists(rootDirectoryPath))
                Directory.CreateDirectory(rootDirectoryPath);

            return rootDirectoryPath;
        }
    }
}
