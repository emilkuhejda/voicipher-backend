using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Payloads.Audio;

namespace Voicipher.Domain.Interfaces.Commands.Audio
{
    public interface ISubmitFileChunkCommand : ICommand<SubmitFileChunkPayload, CommandResult<FileItemOutputModel>>
    {
    }
}
