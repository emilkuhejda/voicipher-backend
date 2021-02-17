using System;

namespace Voicipher.Domain.Models
{
    public record RecognitionFile
    {
        public RecognitionFile()
            : this(Guid.Empty, Guid.Empty, string.Empty)
        {
        }

        public RecognitionFile(Guid userId, Guid audioFileId, string fileName)
        {
            UserId = userId;
            AudioFileId = audioFileId;
            FileName = fileName;
            DateProcessedUtc = DateTime.UtcNow;
        }

        public Guid UserId { get; init; }

        public Guid AudioFileId { get; init; }

        public string FileName { get; init; }

        public DateTime DateProcessedUtc { get; init; }
    }
}
