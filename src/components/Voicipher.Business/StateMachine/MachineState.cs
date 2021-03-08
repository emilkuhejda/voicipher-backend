using System;
using Newtonsoft.Json;
using Voicipher.Business.Extensions;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.StateMachine;
using Voicipher.Domain.Models;

namespace Voicipher.Business.StateMachine
{
    public class MachineState : IMachineState
    {
        public MachineState()
        {
            JobState = JobState.Idle;
            DateCompletedUtc = DateTime.MinValue;
            TranscribedAudioFiles = Array.Empty<TranscribedAudioFile>();
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

        public TranscribedAudioFile[] TranscribedAudioFiles { get; set; }

        public void FromBackgroundJob(BackgroundJob backgroundJob)
        {
            JobId = backgroundJob.Id;
            UserId = backgroundJob.UserId;
            AudioFileId = backgroundJob.AudioFileId;
            Attempt = backgroundJob.Attempt + 1;
            FileName = backgroundJob.GetParameterValue(BackgroundJobParameter.FileName, string.Empty);
        }

        public void FromState(IMachineState stateToRestore)
        {
            JobState = stateToRestore.JobState;
            Attempt = stateToRestore.Attempt;
            FileName = stateToRestore.FileName;
            WavSourceFileName = stateToRestore.WavSourceFileName;
            StateFileName = stateToRestore.StateFileName;
            DateCompletedUtc = stateToRestore.DateCompletedUtc;
            IsRestored = true;
            TranscribedAudioFiles = stateToRestore.TranscribedAudioFiles;
        }

        public void ClearTranscribedAudioFiles()
        {
            TranscribedAudioFiles = Array.Empty<TranscribedAudioFile>();
        }
    }
}
