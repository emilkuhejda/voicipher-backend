using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Domain.Interfaces.Services;

namespace Voicipher.Business.Services
{
    public class FileAccessService : IFileAccessService
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken)
        {
            return File.ReadAllBytesAsync(path, cancellationToken);
        }

        public Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken)
        {
            return File.ReadAllTextAsync(path, cancellationToken);
        }

        public void Delete(string path)
        {
            if (Exists(path))
            {
                File.Delete(path);
            }
        }

        public void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}
