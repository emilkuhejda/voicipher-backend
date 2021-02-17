using System;
using SystemPath = System.IO.Path;

namespace Voicipher.Domain.Models
{
    public record TranscribedAudioFile
    {
        public Guid Id { get; set; }

        public Guid AudioFileId { get; set; }

        public string Path { get; set; }

        public string SourceFileName => string.IsNullOrWhiteSpace(Path) ? string.Empty : SystemPath.GetFileName(Path);

        public int AudioChannels { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public TimeSpan TotalTime { get; set; }
    }
}
