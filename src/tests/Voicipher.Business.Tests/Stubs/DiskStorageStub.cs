using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Tests.Stubs
{
    public class DiskStorageStub : IDiskStorage
    {
        private readonly string _tempDirectory;
        private readonly string _uploadedFilePath;

        public DiskStorageStub()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), "test-data");
            _uploadedFilePath = Path.Combine(_tempDirectory, $"{Guid.NewGuid()}.voc");

            Directory.CreateDirectory(_tempDirectory);
        }

        public async Task<string> UploadAsync(byte[] bytes, CancellationToken cancellationToken)
        {
            await File.WriteAllBytesAsync(_uploadedFilePath, bytes, cancellationToken);
            return _uploadedFilePath;
        }

        public Task<string> UploadAsync(byte[] bytes, DiskStorageSettings diskStorageSettings, CancellationToken cancellationToken)
        {
            return Task.FromResult(diskStorageSettings.FileName);
        }

        public Task<byte[]> ReadAllBytesAsync(FileChunk[] fileChunks, CancellationToken cancellationToken)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var path = Path.Combine(directory, "Samples", "sample.wav");
            return File.ReadAllBytesAsync(path, cancellationToken);
        }

        public void Delete(DiskStorageSettings diskStorageSettings)
        {
        }

        public void DeleteRange(FileChunk[] fileChunks)
        {
        }

        public void DeleteFolder()
        {
        }

        public void DeleteFolder(string folderName)
        {
        }

        public string GetDirectoryPath()
        {
            return GetDirectoryPath(string.Empty);
        }

        public string GetDirectoryPath(string folderName)
        {
            return _tempDirectory;
        }

        public void Clean()
        {
            Directory.Delete(_tempDirectory, true);
        }
    }
}
