using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.Audio;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Utils;

namespace Voicipher.Business.Commands.Audio
{
    public class CreateAudioFileCommand : Command<CreateAudioFileInputModel, CommandResult<AudioFileOutputModel>>, ICreateAudioFileCommand
    {
        private readonly ILogger _logger;

        public CreateAudioFileCommand(ILogger logger)
        {
            _logger = logger.ForContext<CreateAudioFileCommand>();
        }

        protected override Task<CommandResult<AudioFileOutputModel>> Execute(CreateAudioFileInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(parameter.Language) && !SupportedLanguages.IsSupported(parameter.Language))
            {
                _logger.Error($"Language '{parameter.Language}' is not supported.");

                throw new OperationErrorException(ErrorCode.EC200);
            }
        }
    }
}
