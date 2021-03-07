using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IDiskStorage
    {
        Task<string> UploadAsync(byte[] bytes, CancellationToken cancellationToken);

        Task<string> UploadAsync(byte[] bytes, DiskStorageSettings diskStorageSettings, CancellationToken cancellationToken);

        Task<byte[]> ReadAllBytesAsync(FileChunk[] fileChunks, CancellationToken cancellationToken);

        void Delete(DiskStorageSettings diskStorageSettings);

        void DeleteRange(FileChunk[] fileChunks);

        void DeleteFolder();

        void DeleteFolder(string folderName);

        string GetDirectoryPath();

        string GetDirectoryPath(string folderName);
    }
}