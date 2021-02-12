using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.AudioFile;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels.AudioFile;

namespace Voicipher.Domain.Interfaces.Commands.AudioFile
{
    public interface ICreateAudioFileCommand : ICommand<CreateAudioFileInputModel, CommandResult<AudioFileOutputModel>>
    {
    }
}
