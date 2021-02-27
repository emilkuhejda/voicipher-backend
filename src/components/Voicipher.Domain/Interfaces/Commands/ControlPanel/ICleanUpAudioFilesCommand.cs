using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels.ControlPanel;
using Voicipher.Domain.Payloads.ControlPanel;

namespace Voicipher.Domain.Interfaces.Commands.ControlPanel
{
    public interface ICleanUpAudioFilesCommand : ICommand<CleanUpAudioFilesPayload, CommandResult<CleanUpAudioFilesOutputModel>>
    {
    }
}
