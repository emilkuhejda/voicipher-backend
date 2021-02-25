﻿using System;
using System.Collections.Generic;

namespace Voicipher.Domain.Utils
{
    public static class MediaContentTypes
    {
        private static readonly IList<string> Mp3ContentTypes = new List<string>
        {
            "audio/mpeg",
            "audio/mpeg3",
            "audio/x-mpeg-3",
            "video/mpeg",
            "video/x-mpeg"
        };

        public static bool IsUnsupported(string contentType)
        {
            return !contentType.Contains("audio", StringComparison.OrdinalIgnoreCase) || Mp3ContentTypes.Contains(contentType);
        }
    }
}
