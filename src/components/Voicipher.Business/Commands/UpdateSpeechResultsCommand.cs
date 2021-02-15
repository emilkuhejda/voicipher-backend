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
    public class UpdateSpeechResultsCommand : Command<SpeechResultInputModel[], CommandResult<TimeSpanWrapperOutputModel>>, IUpdateSpeechResultsCommand
    {
        protected override async Task<CommandResult<TimeSpanWrapperOutputModel>> Execute(SpeechResultInputModel[] parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            return new CommandResult<TimeSpanWrapperOutputModel>(new TimeSpanWrapperOutputModel(0));
        }
    }
}
