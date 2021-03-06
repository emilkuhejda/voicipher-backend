using System;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Payloads.Audio;

namespace Voicipher.Business.Commands.Audio
{
    public class SubmitFileChunkCommand : Command<SubmitFileChunkPayload, CommandResult<FileItemOutputModel>>, ISubmitFileChunkCommand
    {
        private readonly IAudioService _audioService;
        private readonly IBlobStorage _blobStorage;
        private readonly IDiskStorage _diskStorage;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IFileChunkRepository _fileChunkRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public SubmitFileChunkCommand(
            IAudioService audioService,
            IBlobStorage blobStorage,
            IIndex<StorageLocation, IDiskStorage> index,
            IAudioFileRepository audioFileRepository,
            IFileChunkRepository fileChunkRepository,
            IMapper mapper,
            ILogger logger)
        {
            _audioService = audioService;
            _blobStorage = blobStorage;
            _diskStorage = index[StorageLocation.Chunk];
            _audioFileRepository = audioFileRepository;
            _fileChunkRepository = fileChunkRepository;
            _mapper = mapper;
            _logger = logger.ForContext<SubmitFileChunkCommand>();
        }

        protected override async Task<CommandResult<FileItemOutputModel>> Execute(SubmitFileChunkPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            if (!parameter.Validate().IsValid)
            {
                _logger.Error($"[{userId}] Invalid input data");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            _logger.Verbose($"[{userId}] Start submitting audio file to blob storage");

            var audioFile = await _audioFileRepository.GetAsync(parameter.AudioFileId, cancellationToken);
            if (audioFile == null)
            {
                _logger.Error($"[{userId}] Audio file {parameter.AudioFileId} was not found");

                throw new OperationErrorException(ErrorCode.EC101);
            }

            var tempFilePath = string.Empty;
            var sourceName = string.Empty;
            var fileChunks = await _fileChunkRepository.GetByAudioFileIdAsync(parameter.AudioFileId);
            var isOperationSuccessful = false;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (fileChunks.Length != parameter.ChunksCount)
                {
                    _logger.Error($"[{userId}] Chunks count does not match for request");

                    throw new OperationErrorException(ErrorCode.EC102);
                }

                var audioFileBytes = await _diskStorage.ReadAllBytesAsync(fileChunks, cancellationToken);
                tempFilePath = await _diskStorage.UploadAsync(audioFileBytes, cancellationToken);

                _logger.Verbose($"[{userId}] Audio file was created on destination: {tempFilePath}");

                var audioFileTime = _audioService.GetTotalTime(tempFilePath);
                if (!audioFileTime.HasValue)
                {
                    _logger.Error($"[{userId}] Audio file {audioFile.FileName} is not supported");

                    throw new OperationErrorException(ErrorCode.EC201);
                }

                cancellationToken.ThrowIfCancellationRequested();

                _logger.Verbose($"[{userId}] Start uploading audio file to blob storage");

                var uploadBlobSettings = new UploadBlobSettings(tempFilePath, userId, parameter.AudioFileId);
                sourceName = await _blobStorage.UploadAsync(uploadBlobSettings, cancellationToken);

                _logger.Verbose($"[{userId}] Audio file {sourceName} was uploaded to blob storage. Audio file ID = {audioFile.Id}");

                audioFile.ApplicationId = parameter.ApplicationId;
                audioFile.OriginalSourceFileName = sourceName;
                audioFile.UploadStatus = UploadStatus.Completed;
                audioFile.TotalTime = audioFileTime.Value;
                audioFile.DateUpdatedUtc = DateTime.UtcNow;

                await _fileChunkRepository.SaveAsync(cancellationToken);
                isOperationSuccessful = true;

                _logger.Information($"[{userId}] Audio file {audioFile.Id} was successfully submitted");

                var outputModel = _mapper.Map<FileItemOutputModel>(audioFile);
                return new CommandResult<FileItemOutputModel>(outputModel);
            }
            catch (RequestFailedException ex)
            {
                _logger.Error(ex, $"[{userId}] Blob storage is unavailable. Audio file ID = {parameter.AudioFileId}");

                throw new OperationErrorException(ErrorCode.EC700);
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex, $"[{userId}] Exception occurred during submitting file chunks");

                throw new OperationErrorException(ErrorCode.EC400);
            }
            catch (OperationCanceledException)
            {
                _logger.Warning($"[{userId}] Operation was cancelled");

                throw new OperationErrorException(ErrorCode.EC800);
            }
            finally
            {
                if (!isOperationSuccessful)
                {
                    _logger.Verbose($"[{userId}] Clean audio file from blob storage");

                    var deleteBlobSettings = new DeleteBlobSettings(sourceName, userId, parameter.AudioFileId);
                    await _blobStorage.DeleteFileBlobAsync(deleteBlobSettings, default);
                }

                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);

                    _logger.Verbose($"[{userId}] Audio file was removed on destination: {tempFilePath}");
                }

                _diskStorage.DeleteRange(fileChunks);
                _fileChunkRepository.RemoveRange(fileChunks);
                await _fileChunkRepository.SaveAsync(cancellationToken);

                _logger.Information($"[{userId}] File chunks ({fileChunks.Length}) were deleted for audio file {parameter.AudioFileId}");
            }
        }
    }
}
