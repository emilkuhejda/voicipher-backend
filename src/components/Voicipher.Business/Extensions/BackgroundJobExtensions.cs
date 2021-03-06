using System.Collections.Generic;
using Newtonsoft.Json;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Models;
using Voicipher.Domain.State;

namespace Voicipher.Business.Extensions
{
    public static class BackgroundJobExtensions
    {
        public static T GetParameterValue<T>(this BackgroundJob backgroundJob, BackgroundJobParameter backgroundJobParameter, T defaultValue = default)
        {
            var parameters = JsonConvert.DeserializeObject<Dictionary<BackgroundJobParameter, object>>(backgroundJob.Parameters);
            return parameters.GetValue(backgroundJobParameter, defaultValue);
        }

        public static void FromState(this BackgroundJob backgroundJob, MachineState machineState)
        {
            backgroundJob.JobState = machineState.JobState;
            backgroundJob.Attempt = machineState.Attempt;
            backgroundJob.DateCompletedUtc = machineState.DateCompletedUtc;
        }
    }
}
