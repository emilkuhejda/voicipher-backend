using System.Threading;
using System.Threading.Tasks;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IFileAccessService
    {
        bool Exists(string path);

        Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken);

        void Delete(string path);
    }
}
