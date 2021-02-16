using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.Payloads.Transcription;

namespace Voicipher.Domain.Interfaces.Commands.Transcription
{
    public interface IUpdateRecognitionStateCommand : ICommand<UpdateRecognitionStatePayload, CommandResult>
    {
    }
}
