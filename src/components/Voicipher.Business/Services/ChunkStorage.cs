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
    public class ChunkStorage : IChunkStorage
    {
        private const string UploadedFilesDirectory = "uploaded";
        private const string ChunksDirectory = "chunks";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ChunkStorage(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadAsync(byte[] bytes, CancellationToken cancellationToken)
        {
            var fileName = $"{Guid.NewGuid()}.tmp";
            var rootPath = GetRootPath();
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
            var rootPath = GetRootPath();
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, true);
            }
        }

        private string GetRootPath()
        {
            var rootDirectoryPath = Path.Combine(_webHostEnvironment.WebRootPath, UploadedFilesDirectory, ChunksDirectory);
            if (!Directory.Exists(rootDirectoryPath))
                Directory.CreateDirectory(rootDirectoryPath);

            return rootDirectoryPath;
        }
    }
}
