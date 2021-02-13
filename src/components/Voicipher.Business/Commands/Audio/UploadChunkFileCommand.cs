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
    public class UploadChunkFileCommand : Command<UploadChunkFilePayload, CommandResult<OkOutputModel>>, IUploadChunkFileCommand
    {
        private readonly ILogger _logger;

        public UploadChunkFileCommand(ILogger logger)
        {
            _logger = logger.ForContext<UploadChunkFileCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(UploadChunkFilePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                if (validationResult.Errors.ContainsError(nameof(UploadChunkFilePayload.File), ValidationErrorCodes.ParameterIsNull))
                {
                    _logger.Error("Uploaded file source was not found.");

                    throw new OperationErrorException(ErrorCode.EC100);
                }

                throw new OperationErrorException(ErrorCode.EC600);
            }

            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
