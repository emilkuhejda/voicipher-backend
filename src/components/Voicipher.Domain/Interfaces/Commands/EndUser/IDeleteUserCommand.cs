using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Domain.Interfaces.Commands.EndUser
{
    public interface IDeleteUserCommand : ICommand<string, CommandResult<OkOutputModel>>
    {
    }
}
