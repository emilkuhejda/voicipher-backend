using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads.EndUser;

namespace Voicipher.Domain.Interfaces.Commands.EndUser
{
    public interface IUpdateLanguageCommand : ICommand<UpdateLanguagePayload, CommandResult<OkOutputModel>>
    {
    }
}
