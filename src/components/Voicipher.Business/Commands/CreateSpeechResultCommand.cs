using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Commands
{
    public class CreateSpeechResultCommand : Command<CreateSpeechResultInputModel, CommandResult<OkOutputModel>>, ICreateSpeechResultCommand
    {
        protected override async Task<CommandResult<OkOutputModel>> Execute(CreateSpeechResultInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
