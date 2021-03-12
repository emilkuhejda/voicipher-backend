using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.StateMachine
{
    public interface IStateMachineContext
    {
        AudioFile AudioFile { get; }

        BackgroundJob BackgroundJob { get; }
    }
}
