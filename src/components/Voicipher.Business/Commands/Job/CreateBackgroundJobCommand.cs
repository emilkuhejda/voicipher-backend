using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Job;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads.Job;

namespace Voicipher.Business.Commands.Job
{
    public class CreateBackgroundJobCommand : Command<CreateBackgroundJobPayload, CommandResult<BackgroundJob>>, ICreateBackgroundJobCommand
    {
        protected override async Task<CommandResult<BackgroundJob>> Execute(CreateBackgroundJobPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return new CommandResult<BackgroundJob>(new BackgroundJob());
        }
    }
}
