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
        private const string ContainerName = "voicipher-audio-files";

        private readonly AppSettings _appSettings;

        public BlobStorage(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
        }

        public async Task<string> UploadAsync(UploadBlobSettings uploadBlobSettings)
        {
            var fileName = $"{Guid.NewGuid()}.voc";
            var filePath = Path.Combine(uploadBlobSettings.UserId.ToString(), uploadBlobSettings.AudioFileId.ToString(), fileName);
            var container = await GetContainerClient();
            var client = container.GetBlobClient(filePath);

            using (var fileStream = File.OpenRead(uploadBlobSettings.FilePath))
            {
                await client.UploadAsync(fileStream, true).ConfigureAwait(false);
            }

            return fileName;
        }

        public async Task DeleteAudioFileAsync(BlobSettings blobSettings)
        {
            var container = await GetContainerClient();
            var blobItems = container.GetBlobs()
                .AsPages()
                .SelectMany(x => x.Values)
                .Where(x =>
                    x.Name.Contains(blobSettings.UserId.ToString(), StringComparison.InvariantCulture) &&
                    x.Name.Contains(blobSettings.AudioFileId.ToString(), StringComparison.InvariantCulture));

            foreach (var blobItem in blobItems)
            {
                var client = container.GetBlobClient(blobItem.Name);
                await client.DeleteIfExistsAsync();
            }
        }

        public async Task DeleteFileBlobAsync(DeleteBlobSettings deleteBlobSettings)
        {
            var filePath = Path.Combine(deleteBlobSettings.UserId.ToString(), deleteBlobSettings.AudioFileId.ToString(), deleteBlobSettings.FileName);
            var container = await GetContainerClient();
            var client = container.GetBlobClient(filePath);
            await client.DeleteIfExistsAsync();
        }

        private async Task<BlobContainerClient> GetContainerClient()
        {
            return await GetContainerClient(ContainerName);
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
