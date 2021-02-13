using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Voicipher.Business.Extensions
{
    public static class FormFileExtensions
    {
        public static async Task<byte[]> GetBytesAsync(this IFormFile formFile, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);

                return memoryStream.ToArray();
            }
        }
    }
}
