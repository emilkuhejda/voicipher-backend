using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.State
{
    public class MachineState
    {
        public MachineState()
        {
            JobState = JobState.Idle;
            DateCompletedUtc = DateTime.MinValue;
            TranscribedAudioFiles = new TranscribedAudioFile[0];
        }

        public Guid JobId { get; set; }

        public Guid UserId { get; set; }

        public Guid AudioFileId { get; set; }

        public JobState JobState { get; set; }

        public int Attempt { get; set; }

        public string FileName { get; set; }

        public string WavSourceFileName { get; set; }

        public string StateFileName { get; set; }

        public DateTime DateCompletedUtc { get; set; }

        public bool IsRestored { get; set; }

        [JsonIgnore]
        public string FolderName => AudioFileId.ToString();

        public IEnumerable<TranscribedAudioFile> TranscribedAudioFiles { get; set; }
    }
}
