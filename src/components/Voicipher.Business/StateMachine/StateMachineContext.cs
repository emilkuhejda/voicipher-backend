using Voicipher.Domain.Interfaces.StateMachine;
using Voicipher.Domain.Models;

namespace Voicipher.Business.StateMachine
{
    public class StateMachineContext : IStateMachineContext
    {
        public StateMachineContext(AudioFile audioFile, BackgroundJob backgroundJob)
        {
            AudioFile = audioFile;
            BackgroundJob = backgroundJob;
        }

        public AudioFile AudioFile { get; }

        public BackgroundJob BackgroundJob { get; }
    }
}
