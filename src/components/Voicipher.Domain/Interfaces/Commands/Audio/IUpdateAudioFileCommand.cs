using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.Audio;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels.Audio;

namespace Voicipher.Domain.Interfaces.Commands.Audio
{
    public interface IUpdateAudioFileCommand : ICommand<UpdateAudioFileInputModel, CommandResult<FileItemOutputModel>>
    {
    }
}
