using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.EndUser;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Commands.EndUser
{
    public class AddOrUpdateUserDeviceCommand : Command<User, CommandResult>, IAddOrUpdateUserDeviceCommand
    {
        protected override async Task<CommandResult> Execute(User parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
        }
    }
}
