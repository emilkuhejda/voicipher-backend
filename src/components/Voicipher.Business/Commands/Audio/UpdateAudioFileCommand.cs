using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.Audio;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Audio
{
    public class UpdateAudioFileCommand : Command<UpdateAudioFileInputModel, CommandResult<FileItemOutputModel>>, IUpdateAudioFileCommand
    {
        private readonly ILogger _logger;

        public UpdateAudioFileCommand(ILogger logger)
        {
            _logger = logger;
        }

        protected override async Task<CommandResult<FileItemOutputModel>> Execute(UpdateAudioFileInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            parameter.Language = "fdfd";
            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                if (validationResult.Errors.ContainsError(nameof(UpdateAudioFileInputModel.Language), ValidationErrorCodes.NotSupportedLanguage))
                {
                    _logger.Error($"Language '{parameter.Language}' is not supported.");

                    throw new OperationErrorException(ErrorCode.EC200);
                }

                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            await Task.CompletedTask;

            return new CommandResult<FileItemOutputModel>(new FileItemOutputModel());
        }
    }
}
