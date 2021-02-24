using Microsoft.AspNetCore.Http;
using Voicipher.Domain.Utils;

namespace Voicipher.Domain.Extensions
{
    public static class FormFileExtensions
    {
        public static bool IsContentTypeSupported(this IFormFile formFile)
        {
            return !MediaContentTypes.IsUnsupported(formFile.ContentType);
        }
    }
}
