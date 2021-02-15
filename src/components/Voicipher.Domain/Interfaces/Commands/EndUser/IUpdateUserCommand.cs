using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.EndUser;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Domain.Interfaces.Commands.EndUser
{
    public interface IUpdateUserCommand : ICommand<UpdateUserInputModel, CommandResult<IdentityOutputModel>>
    {
    }
}
