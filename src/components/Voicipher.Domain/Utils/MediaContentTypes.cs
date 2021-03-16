using System;

namespace Voicipher.Domain.Utils
{
    public static class MediaContentTypes
    {
        public static bool IsUnsupported(string contentType)
        {
            return !contentType.Contains("audio", StringComparison.OrdinalIgnoreCase);
        }
    }
}
