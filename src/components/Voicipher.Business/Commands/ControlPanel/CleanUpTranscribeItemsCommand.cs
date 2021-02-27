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
    public class CleanUpTranscribeItemsCommand : Command<CleanUpTranscribeItemsPayload, CommandResult<CleanUpTranscribeItemsOutputModel>>, ICleanUpTranscribeItemsCommand
    {
        protected override async Task<CommandResult<CleanUpTranscribeItemsOutputModel>> Execute(CleanUpTranscribeItemsPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
        }
    }
}
