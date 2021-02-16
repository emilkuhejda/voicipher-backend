using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Payloads;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Transcription
{
    public class CanRunRecognitionCommand : Command<CanRunRecognitionPayload, CommandResult>, ICanRunRecognitionCommand
    {
        protected override async Task<CommandResult> Execute(CanRunRecognitionPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return new CommandResult(new OperationError(string.Empty));
        }
    }
}
