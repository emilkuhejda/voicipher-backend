using System;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Utils
{
    public record SpeechRecognizeConfig
    {
        public SpeechRecognizeConfig(AudioFile audioFile)
        {
            AudioFileId = audioFile.Id;
            UserId = audioFile.UserId;
            Language = audioFile.Language;
            IsPhoneCall = audioFile.IsPhoneCall;
        }

        public Guid AudioFileId { get; }

        public Guid UserId { get; }

        public string Language { get; }

        public bool IsPhoneCall { get; }
    }
}
