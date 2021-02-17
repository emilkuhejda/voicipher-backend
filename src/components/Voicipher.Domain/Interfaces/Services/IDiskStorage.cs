﻿using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IDiskStorage
    {
        Task<string> UploadAsync(byte[] bytes, CancellationToken cancellationToken);

        void RemoveRange(FileChunk[] fileChunks);

        Task<byte[]> ReadAllBytesAsync(FileChunk[] fileChunks, CancellationToken cancellationToken);

        void RemoveTemporaryFolder();

        string GetDirectoryPath();
    }
}