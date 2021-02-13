using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IBlobStorage
    {
        Task<string> UploadAsync(UploadBlobSettings uploadBlobSettings);

        Task DeleteContainer(BlobSettings blobSettings);

        Task DeleteAudioFileAsync(BlobSettings blobSettings);

        Task DeleteFileBlobAsync(DeleteBlobSettings deleteBlobSettings);
    }
}
