using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.Payloads;

namespace Voicipher.Domain.Interfaces.Commands.Transcription
{
    public interface ICanRunRecognitionCommand : ICommand<CanRunRecognitionPayload, CommandResult>
    {
    }
}
