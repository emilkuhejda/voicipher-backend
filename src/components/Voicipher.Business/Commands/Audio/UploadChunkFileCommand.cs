using System;
using System.IO;
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
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Audio
{
    public class UploadChunkFileCommand : Command<UploadChunkFilePayload, CommandResult<OkOutputModel>>, IUploadChunkFileCommand
    {
        private readonly IChunkStorage _chunkStorage;
        private readonly ILogger _logger;

        public UploadChunkFileCommand(
            IChunkStorage chunkStorage,
            ILogger logger)
        {
            _chunkStorage = chunkStorage;
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

            string filePath = string.Empty;
            try
            {
                var uploadedFileSource = await parameter.File.GetBytesAsync(cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();

                var tempFileName = $"{Guid.NewGuid()}.tmp";
                filePath = await _chunkStorage.UploadAsync(uploadedFileSource, tempFileName, cancellationToken);

                _logger.Information($"File chunk for file item '{parameter.FileItemId}' was uploaded.");

                return new CommandResult<OkOutputModel>(new OkOutputModel());
            }
            catch (OperationCanceledException)
            {
                throw new OperationErrorException(ErrorCode.EC800);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
