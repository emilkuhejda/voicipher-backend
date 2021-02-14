using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.Authentication;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels.MetaData;

namespace Voicipher.Domain.Interfaces.Commands.Authentication
{
    public interface IAuthenticateUserCommand : ICommand<AuthenticateUserInputModel, CommandResult<AdministratorOutputModel>>
    {
    }
}
