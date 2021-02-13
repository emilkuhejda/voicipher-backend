using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Payloads.Audio;

namespace Voicipher.Domain.Interfaces.Commands.Audio
{
    public interface ICreateAudioFileCommand : ICommand<CreateAudioFilePayload, CommandResult<AudioFileOutputModel>>
    {
    }
}
