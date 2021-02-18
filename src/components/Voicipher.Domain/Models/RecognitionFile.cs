using System;

namespace Voicipher.Domain.Models
{
    public record RecognitionFile
    {
        public RecognitionFile()
            : this(Guid.Empty, Guid.Empty, Guid.NewGuid(), string.Empty)
        {
        }

        public RecognitionFile(Guid userId, Guid audioFileId, string fileName)
            : this(userId, audioFileId, Guid.NewGuid(), fileName)
        {
        }

        public RecognitionFile(Guid userId, Guid audioFileId, Guid jobId, string fileName)
        {
            UserId = userId;
            AudioFileId = audioFileId;
            JobId = jobId;
            FileName = fileName;
            DateProcessedUtc = DateTime.UtcNow;
        }

        public Guid UserId { get; init; }

        public Guid AudioFileId { get; init; }

        public Guid JobId { get; init; }

        public string FileName { get; init; }

        public DateTime DateProcessedUtc { get; init; }
    }
}
