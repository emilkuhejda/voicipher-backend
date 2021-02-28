using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads.Notifications;

namespace Voicipher.Domain.Interfaces.Commands.Notifications
{
    public interface ICreateInformationMessageCommand : ICommand<InformationMessagePayload, CommandResult<InformationMessageOutputModel>>
    {
    }
}
