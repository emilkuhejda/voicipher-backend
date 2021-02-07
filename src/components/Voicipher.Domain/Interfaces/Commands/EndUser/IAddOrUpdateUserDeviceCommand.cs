using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.Models;

namespace Voicipher.Domain.Interfaces.Commands.EndUser
{
    public interface IAddOrUpdateUserDeviceCommand : ICommand<User, CommandResult>
    {
    }
}
