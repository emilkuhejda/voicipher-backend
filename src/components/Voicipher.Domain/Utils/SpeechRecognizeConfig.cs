using System;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Utils
{
    public class SpeechRecognizeConfig
    {
        public SpeechRecognizeConfig(AudioFile audioFile)
        {
            UserId = audioFile.UserId;
            Language = audioFile.Language;
            IsPhoneCall = audioFile.IsPhoneCall;
        }

        public Guid UserId { get; }

        public string Language { get; }

        public bool IsPhoneCall { get; }
    }
}
