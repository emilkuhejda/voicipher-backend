using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.AudioFile;
using Voicipher.Domain.Interfaces.Commands.AudioFile;
using Voicipher.Domain.OutputModels.AudioFile;

namespace Voicipher.Business.Commands.AudioFile
{
    public class CreateAudioFileCommand : Command<CreateAudioFileInputModel, CommandResult<AudioFileOutputModel>>, ICreateAudioFileCommand
    {
        protected override Task<CommandResult<AudioFileOutputModel>> Execute(CreateAudioFileInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();

            throw new OperationErrorException(ErrorCode.EC100);
        }
    }
}
