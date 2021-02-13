using System.IO;
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

        public async Task<string> UploadAsync(byte[] bytes, string outputFileName, CancellationToken cancellationToken)
        {
            var rootPath = GetRootPath();
            var filePath = Path.Combine(rootPath, outputFileName);
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

        private string GetRootPath()
        {
            var rootDirectoryPath = Path.Combine(_webHostEnvironment.WebRootPath, UploadedFilesDirectory, ChunksDirectory);
            if (!Directory.Exists(rootDirectoryPath))
                Directory.CreateDirectory(rootDirectoryPath);

            return rootDirectoryPath;
        }
    }
}
