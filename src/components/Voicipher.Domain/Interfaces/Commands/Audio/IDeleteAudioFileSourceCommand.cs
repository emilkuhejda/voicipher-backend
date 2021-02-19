using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.Payloads.Audio;

namespace Voicipher.Domain.Interfaces.Commands.Audio
{
    public interface IDeleteAudioFileSourceCommand : ICommand<DeleteAudioFileSourcePayload, CommandResult>
    {
    }
}
