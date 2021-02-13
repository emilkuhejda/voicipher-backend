using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Services
{
    public class BlobStorage : IBlobStorage
    {
        private readonly AppSettings _appSettings;

        public BlobStorage(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
        }

        public async Task<string> UploadAsync(UploadBlobSettings uploadBlobSettings)
        {
            var fileName = $"{Guid.NewGuid()}.voc";
            var filePath = Path.Combine(uploadBlobSettings.AudioFileId, fileName);
            var container = await GetContainerClient(uploadBlobSettings.ContainerName);
            var client = container.GetBlobClient(filePath);

            using (var fileStream = File.OpenRead(uploadBlobSettings.FilePath))
            {
                await client.UploadAsync(fileStream, true).ConfigureAwait(false);
            }

            return fileName;
        }

        public async Task DeleteContainer(BlobSettings blobSettings)
        {
            var container = await GetContainerClient(blobSettings.ContainerName);
            await container.DeleteIfExistsAsync();
        }

        public async Task DeleteAudioFileAsync(BlobSettings blobSettings)
        {
            var container = await GetContainerClient(blobSettings.ContainerName);
            var blobItems = container.GetBlobs()
                .AsPages()
                .SelectMany(x => x.Values)
                .Where(x => x.Name.Contains(blobSettings.AudioFileId, StringComparison.InvariantCulture));

            foreach (var blobItem in blobItems)
            {
                var client = container.GetBlobClient(blobItem.Name);
                await client.DeleteIfExistsAsync();
            }
        }

        public async Task DeleteFileBlobAsync(DeleteBlobSettings deleteBlobSettings)
        {
            var filePath = Path.Combine(deleteBlobSettings.AudioFileId, deleteBlobSettings.FileName);
            var container = await GetContainerClient(deleteBlobSettings.ContainerName);
            var client = container.GetBlobClient(filePath);
            await client.DeleteIfExistsAsync();
        }

        private async Task<BlobContainerClient> GetContainerClient(string containerName)
        {
            var blobServiceClient = new BlobServiceClient(_appSettings.AzureStorageAccount.ConnectionString);
            var container = blobServiceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();

            return container;
        }
    }
}
