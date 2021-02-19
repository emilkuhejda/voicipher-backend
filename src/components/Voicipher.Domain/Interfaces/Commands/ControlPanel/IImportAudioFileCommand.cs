using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.Payloads.ControlPanel;

namespace Voicipher.Domain.Interfaces.Commands.ControlPanel
{
    public interface IImportAudioFileCommand : ICommand<ImportAudioFilePayload, CommandResult<int>>
    {
    }
}
