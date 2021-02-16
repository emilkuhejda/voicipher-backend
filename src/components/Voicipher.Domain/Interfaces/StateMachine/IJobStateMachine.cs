using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.StateMachine
{
    public interface IJobStateMachine
    {
        void DoInit(BackgroundJob backgroundJob);

        void DoValidation();
    }
}
