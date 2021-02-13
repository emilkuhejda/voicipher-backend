using System;

namespace Voicipher.Domain.Models
{
    public class RecognitionWordInfo
    {
        public string Word { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int SpeakerTag { get; set; }
    }
}
