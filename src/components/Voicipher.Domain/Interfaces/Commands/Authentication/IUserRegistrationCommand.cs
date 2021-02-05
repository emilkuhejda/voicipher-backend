using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.Authentication;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels.Authentication;

namespace Voicipher.Domain.Interfaces.Commands.Authentication
{
    public interface IUserRegistrationCommand : ICommand<UserRegistrationInputModel, CommandResult<UserRegistrationOutputModel>>
    {
    }
}
