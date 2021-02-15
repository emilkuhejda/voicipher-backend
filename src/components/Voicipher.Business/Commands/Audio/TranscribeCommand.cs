using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Audio
{
    public class TranscribeCommand : Command<TranscribePayload, CommandResult<OkOutputModel>>, ITranscribeCommand
    {
        private readonly ILogger _logger;

        public TranscribeCommand(ILogger logger)
        {
            _logger = logger.ForContext<TranscribeCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(TranscribePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                if (validationResult.Errors.ContainsError(nameof(CreateAudioFilePayload.Language), ValidationErrorCodes.NotSupportedLanguage))
                {
                    _logger.Error($"Language '{parameter.Language}' is not supported.");

                    throw new OperationErrorException(ErrorCode.EC200);
                }

                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            await Task.CompletedTask;

            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
