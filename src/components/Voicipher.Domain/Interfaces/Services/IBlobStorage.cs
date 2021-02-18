using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IBlobStorage
    {
        Task<byte[]> GetAsync(GetBlobSettings blobSettings, CancellationToken cancellationToken);

        Task<string> UploadAsync(UploadBlobSettings blobSettings, CancellationToken cancellationToken);

        Task DeleteContainer(BlobContainerSettings blobSettings, CancellationToken cancellationToken);

        Task DeleteAudioFileAsync(BlobSettings blobSettings, CancellationToken cancellationToken);

        Task DeleteTranscribedFiles(DeleteBlobSettings blobSettings, CancellationToken cancellationToken);

        Task DeleteFileBlobAsync(DeleteBlobSettings blobSettings, CancellationToken cancellationToken);
    }
}
