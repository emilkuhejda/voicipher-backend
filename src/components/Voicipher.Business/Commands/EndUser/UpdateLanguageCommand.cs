using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.EndUser;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads.EndUser;

namespace Voicipher.Business.Commands.EndUser
{
    public class UpdateLanguageCommand : Command<UpdateLanguagePayload, CommandResult<OkOutputModel>>, IUpdateLanguageCommand
    {
        protected override Task<CommandResult<OkOutputModel>> Execute(UpdateLanguagePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
        }
    }
}
