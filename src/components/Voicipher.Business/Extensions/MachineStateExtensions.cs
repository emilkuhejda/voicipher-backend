using Voicipher.Domain.Enums;
using Voicipher.Domain.Models;
using Voicipher.Domain.State;

namespace Voicipher.Business.Extensions
{
    public static class MachineStateExtensions
    {
        public static void FromBackgroundJob(this MachineState machineState, BackgroundJob backgroundJob)
        {
            machineState.JobId = backgroundJob.Id;
            machineState.UserId = backgroundJob.UserId;
            machineState.AudioFileId = backgroundJob.AudioFileId;
            machineState.Attempt = backgroundJob.Attempt + 1;
            machineState.FileName = backgroundJob.GetParameterValue(BackgroundJobParameter.FileName, string.Empty);
        }

        public static void FromState(this MachineState machineState, MachineState stateToRestore)
        {
            machineState.JobState = stateToRestore.JobState;
            machineState.Attempt = stateToRestore.Attempt;
            machineState.FileName = stateToRestore.FileName;
            machineState.WavSourceFileName = stateToRestore.WavSourceFileName;
            machineState.StateFileName = stateToRestore.StateFileName;
            machineState.DateCompletedUtc = stateToRestore.DateCompletedUtc;
            machineState.IsRestored = true;
            machineState.TranscribedAudioFiles = stateToRestore.TranscribedAudioFiles;
        }
    }
}
