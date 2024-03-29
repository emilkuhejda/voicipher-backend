﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Settings;
using Voicipher.Domain.Utils;

namespace Voicipher.Business.Services
{
    public class BlobStorage : IBlobStorage
    {
        private readonly AppSettings _appSettings;

        public BlobStorage(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
        }

        public async Task<bool> Exists(GetBlobSettings blobSettings, CancellationToken cancellationToken)
        {
            var container = await GetContainerClient(blobSettings.ContainerName, cancellationToken);
            var filePath = Path.Combine(blobSettings.AudioFileId, blobSettings.FileName);
            var client = container.GetBlobClient(filePath);
            return await client.ExistsAsync(cancellationToken);
        }

        public async Task<byte[]> GetAsync(GetBlobSettings blobSettings, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine(blobSettings.AudioFileId, blobSettings.FileName ?? string.Empty);
            var container = await GetContainerClient(blobSettings.ContainerName, cancellationToken);
            var client = container.GetBlobClient(filePath);
            var exists = await client.ExistsAsync(cancellationToken);
            if (!exists)
                throw new BlobNotExistsException();

            using (var memoryStream = new MemoryStream())
            {
                await client.DownloadToAsync(memoryStream, cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<string> UploadAsync(UploadBlobSettings blobSettings, CancellationToken cancellationToken)
        {
            var fileName = string.IsNullOrWhiteSpace(blobSettings.FileName) ? $"{Guid.NewGuid()}{MimeTypes.VocExtension}" : blobSettings.FileName;
            var filePath = Path.Combine(blobSettings.AudioFileId, fileName);
            var container = await GetContainerClient(blobSettings.ContainerName, cancellationToken);
            var client = container.GetBlobClient(filePath);

            using (var fileStream = File.OpenRead(blobSettings.FilePath))
            {
                await client.UploadAsync(
                    fileStream,
                    httpHeaders: new BlobHttpHeaders { ContentType = blobSettings.ContentType },
                    metadata: blobSettings.Metadata,
                    cancellationToken: cancellationToken);
            }

            return fileName;
        }

        public async Task DeleteContainer(BlobContainerSettings blobSettings, CancellationToken cancellationToken)
        {
            var container = await GetContainerClient(blobSettings.ContainerName, cancellationToken);
            await container.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }

        public async Task DeleteAudioFileAsync(BlobSettings blobSettings, CancellationToken cancellationToken)
        {
            var container = await GetContainerClient(blobSettings.ContainerName, cancellationToken);
            var blobItems = container.GetBlobs()
                .AsPages()
                .SelectMany(x => x.Values)
                .Where(x => x.Name.Contains(blobSettings.AudioFileId, StringComparison.OrdinalIgnoreCase));

            foreach (var blobItem in blobItems)
            {
                var client = container.GetBlobClient(blobItem.Name);
                await client.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            }
        }

        public async Task DeleteTranscribedFiles(DeleteBlobSettings blobSettings, CancellationToken cancellationToken)
        {
            var container = await GetContainerClient(blobSettings.ContainerName, cancellationToken);
            var blobItems = container.GetBlobs()
                .AsPages()
                .SelectMany(x => x.Values)
                .Where(x =>
                    x.Name.Contains(blobSettings.AudioFileId, StringComparison.OrdinalIgnoreCase) &&
                    !x.Name.Contains(blobSettings.FileName, StringComparison.OrdinalIgnoreCase));

            foreach (var blobItem in blobItems)
            {
                var client = container.GetBlobClient(blobItem.Name);
                var properties = await client.GetPropertiesAsync(cancellationToken: cancellationToken);
                if (properties.Value.Metadata.ContainsKey(BlobMetadata.TranscribedAudioFile))
                {
                    await client.DeleteIfExistsAsync(cancellationToken: cancellationToken);
                }
            }
        }

        public async Task DeleteFileBlobAsync(DeleteBlobSettings blobSettings, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine(blobSettings.AudioFileId, blobSettings.FileName);
            var container = await GetContainerClient(blobSettings.ContainerName, cancellationToken);
            var client = container.GetBlobClient(filePath);
            await client.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }

        private async Task<BlobContainerClient> GetContainerClient(string containerName, CancellationToken cancellationToken)
        {
            var blobServiceClient = new BlobServiceClient(_appSettings.AzureStorageAccount.ConnectionString);
            var container = blobServiceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            return container;
        }
    }
}
