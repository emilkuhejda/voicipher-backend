using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads;

namespace Voicipher.Domain.Interfaces.Commands
{
    public interface ICreateUserSubscriptionCommand : ICommand<CreateUserSubscriptionPayload, CommandResult<TimeSpanWrapperOutputModel>>
    {
    }
}
