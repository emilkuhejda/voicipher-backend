using System;

namespace Voicipher.Domain.Models
{
    public record BlobFile
    {
        public BlobFile(Guid userId, Guid audioFileId, string filePath)
        {
            UserId = userId;
            AudioFileId = audioFileId;
            FilePath = filePath;
        }

        public Guid UserId { get; }

        public Guid AudioFileId { get; }

        public string FilePath { get; }
    }
}
