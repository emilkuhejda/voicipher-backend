using System;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Common.Utils;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Payloads.Audio;

namespace Voicipher.Business.Commands.Audio
{
    public class SubmitFileChunkCommand : Command<SubmitFileChunkPayload, CommandResult<FileItemOutputModel>>, ISubmitFileChunkCommand
    {
        private readonly IAudioService _audioService;
        private readonly IChunkStorage _chunkStorage;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IFileChunkRepository _fileChunkRepository;
        private readonly ILogger _logger;

        public SubmitFileChunkCommand(
            IAudioService audioService,
            IChunkStorage chunkStorage,
            IAudioFileRepository audioFileRepository,
            IFileChunkRepository fileChunkRepository,
            ILogger logger)
        {
            _audioService = audioService;
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

            var tempFilePath = string.Empty;
            var fileChunks = await _fileChunkRepository.GetByAudioFileIdAsync(parameter.AudioFileId);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (fileChunks.Length != parameter.ChunksCount)
                {
                    _logger.Error($"Chunks count does not match. [{userId}]");

                    throw new OperationErrorException(ErrorCode.EC102);
                }

                var audioFileBytes = await _chunkStorage.ReadAllBytesAsync(fileChunks, cancellationToken);
                tempFilePath = await _chunkStorage.UploadAsync(audioFileBytes, cancellationToken);

                _logger.Information($"Audio file was created on destination: '{tempFilePath}'.");

                var audioFileTime = _audioService.GetTotalTime(fileChunks[0].Path);
                if (!audioFileTime.HasValue)
                {
                    _logger.Error($"Audio file '{audioFile.FileName}' is not supported. [{userId}]");

                    throw new OperationErrorException(ErrorCode.EC201);
                }

                cancellationToken.ThrowIfCancellationRequested();

                var sourceName = $"{Guid.NewGuid()}.voc";

                audioFile.ApplicationId = parameter.ApplicationId;
                audioFile.OriginalSourceFileName = sourceName;
                audioFile.UploadStatus = UploadStatus.Completed;
                audioFile.TotalTime = audioFileTime.Value;
                audioFile.DateUpdatedUtc = DateTime.UtcNow;

                await _fileChunkRepository.SaveAsync(cancellationToken);

                _logger.Information($"Audio file '{audioFile.Id}' was successfully submitted. [{userId}]");

                return new CommandResult<FileItemOutputModel>(new FileItemOutputModel());
            }
            catch (DbUpdateException ex)
            {
                _logger.Error($"Exception occurred during submitting file chunks. Message: {ex.Message}. [{userId}]");
                _logger.Error(ExceptionFormatter.FormatException(ex));

                throw new OperationErrorException(ErrorCode.EC400);
            }
            catch (OperationCanceledException)
            {
                _logger.Information("Operation was cancelled.");

                throw new OperationErrorException(ErrorCode.EC800);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);

                    _logger.Information($"Audio file was removed on destination: '{tempFilePath}'.");
                }

                _chunkStorage.RemoveRange(fileChunks);
                _fileChunkRepository.RemoveRange(fileChunks);
                await _fileChunkRepository.SaveAsync(cancellationToken);

                _logger.Information($"File chunks ({fileChunks.Length}) were deleted for audio file '{parameter.AudioFileId}'.");
            }
        }
    }
}
