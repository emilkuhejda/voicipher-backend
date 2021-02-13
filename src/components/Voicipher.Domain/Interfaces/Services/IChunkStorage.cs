using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IChunkStorage
    {
        Task<string> UploadAsync(byte[] bytes, string outputFileName, CancellationToken cancellationToken);

        void RemoveRange(FileChunk[] fileChunks);
    }
}
