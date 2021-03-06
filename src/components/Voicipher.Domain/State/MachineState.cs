using System;
using System.Collections.Generic;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.State
{
    public class MachineState
    {
        public MachineState()
        {
            JobState = JobState.Idle;
            TranscribedAudioFiles = new TranscribedAudioFile[0];
        }

        public Guid JobId { get; set; }

        public JobState JobState { get; set; }

        public int Attempt { get; set; }

        public IEnumerable<TranscribedAudioFile> TranscribedAudioFiles { get; set; }
    }
}
