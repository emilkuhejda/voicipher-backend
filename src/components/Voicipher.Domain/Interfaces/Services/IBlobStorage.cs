using System.Threading.Tasks;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IBlobStorage
    {
        Task<string> UploadAsync(BlobFile blobFile);
    }
}
