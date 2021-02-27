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
            return UploadAsync(bytes, new UploadSettings(), cancellationToken);
        }

        public async Task<string> UploadAsync(byte[] bytes, UploadSettings uploadSettings, CancellationToken cancellationToken)
        {
            var fileName = string.IsNullOrWhiteSpace(uploadSettings.FileName) ? $"{Guid.NewGuid()}.tmp" : uploadSettings.FileName;
            var rootPath = GetDirectoryPath(uploadSettings.FolderName ?? string.Empty);
            var filePath = Path.Combine(rootPath, fileName);
            await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);

            return filePath;
        }

        public void RemoveRange(FileChunk[] fileChunks)
        {
            foreach (var fileChunk in fileChunks)
            {
                if (File.Exists(fileChunk.Path))
                {
                    File.Delete(fileChunk.Path);
                }
            }
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

        public void RemoveTemporaryFolder()
        {
            var rootPath = GetDirectoryPath();
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
