using System;
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
using Voicipher.Domain.Interfaces.Infrastructure;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Payloads.Audio;

namespace Voicipher.Business.Commands.Audio
{
    public class SubmitFileChunkCommand : Command<SubmitFileChunkPayload, CommandResult<FileItemOutputModel>>, ISubmitFileChunkCommand
    {
        private readonly IChunkStorage _chunkStorage;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IFileChunkRepository _fileChunkRepository;
        private readonly ILogger _logger;

        public SubmitFileChunkCommand(
            IChunkStorage chunkStorage,
            IAudioFileRepository audioFileRepository,
            IFileChunkRepository fileChunkRepository,
            ILogger logger)
        {
            _chunkStorage = chunkStorage;
            _audioFileRepository = audioFileRepository;
            _fileChunkRepository = fileChunkRepository;
            _logger = logger.ForContext<SubmitFileChunkCommand>();
        }

        protected override async Task<CommandResult<FileItemOutputModel>> Execute(SubmitFileChunkPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (!parameter.Validate().IsValid)
            {
                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            var userId = principal.GetNameIdentifier();
            var audioFile = await _audioFileRepository.GetAsync(parameter.AudioFileId, cancellationToken);
            if (audioFile == null)
            {
                _logger.Error($"Audio file '{parameter.AudioFileId}' was not found. [{userId}]");

                throw new OperationErrorException(ErrorCode.EC101);
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fileChunks = await _fileChunkRepository.GetByAudioFileIdAsync(parameter.AudioFileId);

                if (fileChunks.Length != parameter.ChunksCount)
                {
                    _logger.Error($"Chunks count does not match. [{userId}]");

                    throw new OperationErrorException(ErrorCode.EC102);
                }

                var uploadedFileSource = await _chunkStorage.ReadAllBytesAsync(fileChunks, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                return new CommandResult<FileItemOutputModel>(new FileItemOutputModel());
            }
            catch (OperationCanceledException)
            {
                _logger.Information("Operation was cancelled.");

                throw new OperationErrorException(ErrorCode.EC800);
            }
        }
    }
}
