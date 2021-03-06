using System;

namespace Voicipher.Domain.Models
{
    public record RecognitionFile
    {
        public RecognitionFile()
        {
            JobId = Guid.Empty;
            UserId = Guid.Empty;
            AudioFileId = Guid.Empty;
            FileName = string.Empty;
            DateProcessedUtc = DateTime.MinValue;
        }

        public RecognitionFile(AudioFile audioFile)
        {
            JobId = Guid.NewGuid();
            UserId = audioFile.UserId;
            AudioFileId = audioFile.Id;
            FileName = audioFile.FileName;
            DateProcessedUtc = DateTime.UtcNow;
        }

        public Guid JobId { get; init; }

        public Guid UserId { get; init; }

        public Guid AudioFileId { get; init; }

        public string FileName { get; init; }

        public DateTime DateProcessedUtc { get; init; }
    }
}
