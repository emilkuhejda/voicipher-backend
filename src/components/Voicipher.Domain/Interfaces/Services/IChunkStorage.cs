using System.Threading;
using System.Threading.Tasks;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IChunkStorage
    {
        Task<string> UploadAsync(byte[] bytes, string outputFileName, CancellationToken cancellationToken);
    }
}
