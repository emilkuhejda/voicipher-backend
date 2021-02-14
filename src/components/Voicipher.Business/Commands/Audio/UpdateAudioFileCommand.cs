using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.Audio;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.OutputModels.Audio;

namespace Voicipher.Business.Commands.Audio
{
    public class UpdateAudioFileCommand : Command<UpdateAudioFileInputModel, CommandResult<FileItemOutputModel>>, IUpdateAudioFileCommand
    {
        protected override async Task<CommandResult<FileItemOutputModel>> Execute(UpdateAudioFileInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            return new CommandResult<FileItemOutputModel>(new FileItemOutputModel());
        }
    }
}
