using Microsoft.AspNetCore.StaticFiles;
using Voicipher.Domain.Utils;

namespace Voicipher.Business.Utils
{
    public static class ContentTypeHelper
    {
        private const string DefaultContentType = "application/octet-stream";
        private const string WavContentType = "audio/wav";

        public static string GetContentType(string subPath)
        {
            var contentTypeProvider = new FileExtensionContentTypeProvider();
            contentTypeProvider.Mappings.Add(MimeTypes.VocExtension, WavContentType);
            if (!contentTypeProvider.TryGetContentType(subPath, out var contentType))
            {
                return DefaultContentType;
            }

            return contentType;
        }
    }
}
