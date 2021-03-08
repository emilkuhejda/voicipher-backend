using System;
using Newtonsoft.Json;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.StateMachine
{
    public interface IMachineState
    {
        public Guid JobId { get; }

        public Guid UserId { get; }

        public Guid AudioFileId { get; }

        public JobState JobState { get; }

        public int Attempt { get; }

        public string FileName { get; }

        public string WavSourceFileName { get; }

        public string StateFileName { get; }

        public DateTime DateCompletedUtc { get; }

        public bool IsRestored { get; }

        public TranscribedAudioFile[] TranscribedAudioFiles { get; }
    }
}
