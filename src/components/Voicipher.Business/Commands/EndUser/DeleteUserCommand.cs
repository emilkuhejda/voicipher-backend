using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.EndUser;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Commands.EndUser
{
    public class DeleteUserCommand : Command<string, CommandResult<OkOutputModel>>, IDeleteUserCommand
    {
        protected override Task<CommandResult<OkOutputModel>> Execute(string parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
