using System;
using System.IO;
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

        public async Task<string> UploadAsync(BlobFile blobFile)
        {
            var fileName = $"{Guid.NewGuid()}.voc";
            var filePath = Path.Combine(blobFile.UserId.ToString(), blobFile.AudioFileId.ToString(), fileName);
            var container = await GetContainerClient();
            var client = container.GetBlobClient(filePath);

            using (var fileStream = File.OpenRead(blobFile.FilePath))
            {
                await client.UploadAsync(fileStream, true).ConfigureAwait(false);
            }

            return fileName;
        }

        private async Task<BlobContainerClient> GetContainerClient()
        {
            var blobServiceClient = new BlobServiceClient(_appSettings.AzureStorageAccount.ConnectionString);
            var container = blobServiceClient.GetBlobContainerClient(ContainerName);
            await container.CreateIfNotExistsAsync();

            return container;
        }
    }
}
