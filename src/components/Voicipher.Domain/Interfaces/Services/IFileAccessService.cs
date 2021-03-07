using System.Threading;
using System.Threading.Tasks;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IFileAccessService
    {
        bool Exists(string path);

        bool DirectoryExists(string path);

        string[] GetFiles(string path);

        Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken);

        Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken);

        void Delete(string path);

        void DeleteDirectory(string path);
    }
}
