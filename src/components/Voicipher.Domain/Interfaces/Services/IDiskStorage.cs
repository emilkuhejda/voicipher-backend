﻿using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IDiskStorage
    {
        Task<string> UploadAsync(byte[] bytes, CancellationToken cancellationToken);

        Task<string> UploadAsync(byte[] bytes, UploadSettings uploadSettings, CancellationToken cancellationToken);

        Task<byte[]> ReadAllBytesAsync(FileChunk[] fileChunks, CancellationToken cancellationToken);

        void DeleteRange(FileChunk[] fileChunks);

        void DeleteTemporaryFolder();

        string GetDirectoryPath();

        string GetDirectoryPath(string folderName);
    }
}