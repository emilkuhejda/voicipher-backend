using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IBlobStorage
    {
        Task<byte[]> GetAsync(GetBlobSettings blobSettings);

        Task<string> UploadAsync(UploadBlobSettings blobSettings);

        Task DeleteContainer(BlobSettings blobSettings);

        Task DeleteAudioFileAsync(BlobSettings blobSettings);

        Task DeleteFileBlobAsync(DeleteBlobSettings blobSettings);
    }
}
