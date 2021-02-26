using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.ControlPanel;
using Voicipher.Domain.OutputModels.ControlPanel;
using Voicipher.Domain.Payloads.ControlPanel;

namespace Voicipher.Business.Commands.ControlPanel
{
    public class CleanUpAudioFilesCommand : Command<CleanUpAudioFilesPayload, CommandResult<CleanUpAudioFilesOutputModel>>, ICleanUpAudioFilesCommand
    {
        protected override async Task<CommandResult<CleanUpAudioFilesOutputModel>> Execute(CleanUpAudioFilesPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
