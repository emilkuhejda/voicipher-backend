using System;

namespace Voicipher.Domain.Models
{
    public record TranscribeAudioFile
    {
        public Guid Id { get; set; }

        public Guid AudioFileId { get; set; }

        public string Path { get; set; }

        public int AudioChannels { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public TimeSpan TotalTime { get; set; }
    }
}
