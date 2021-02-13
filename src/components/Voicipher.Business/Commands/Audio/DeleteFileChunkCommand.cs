using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Common.Utils;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads.Audio;

namespace Voicipher.Business.Commands.Audio
{
    public class DeleteFileChunkCommand : Command<DeleteFileChunkPayload, CommandResult<OkOutputModel>>, IDeleteFileChunkCommand
    {
        private readonly IFileChunkRepository _fileChunkRepository;
        private readonly IChunkStorage _chunkStorage;
        private readonly ILogger _logger;

        public DeleteFileChunkCommand(
            IFileChunkRepository fileChunkRepository,
            IChunkStorage chunkStorage,
            ILogger logger)
        {
            _fileChunkRepository = fileChunkRepository;
            _chunkStorage = chunkStorage;
            _logger = logger.ForContext<DeleteFileChunkCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(DeleteFileChunkPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            try
            {
                var fileChunks = await _fileChunkRepository.GetByAudioFileIdAsync(parameter.AudioFileId);

                _chunkStorage.RemoveRange(fileChunks);
                _fileChunkRepository.RemoveRange(fileChunks);
                await _fileChunkRepository.SaveAsync(cancellationToken);

                _logger.Information($"File chunks ({fileChunks.Length}) were deleted for audio file '{parameter.AudioFileId}'.");

                return new CommandResult<OkOutputModel>(new OkOutputModel());
            }
            catch (Exception ex)
            {
                _logger.Error("Operation error.");
                _logger.Error(ExceptionFormatter.FormatException(ex));

                throw new OperationErrorException(ErrorCode.EC603);
            }
        }
    }
}
