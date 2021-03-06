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
    }
}
