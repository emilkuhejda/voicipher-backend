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
using Voicipher.Business.Utils;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Audio
{
    public class UploadAudioFileCommand : Command<UploadAudioFilePayload, CommandResult<FileItemOutputModel>>, IUploadAudioFileCommand
    {
        private readonly IAudioService _audioService;
        private readonly IMessageCenterService _messageCenterService;
        private readonly IBlobStorage _blobStorage;
        private readonly IDiskStorage _diskStorage;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UploadAudioFileCommand(
            IAudioService audioService,
            IMessageCenterService messageCenterService,
            IBlobStorage blobStorage,
            IIndex<StorageLocation, IDiskStorage> index,
            IAudioFileRepository audioFileRepository,
            IMapper mapper,
            ILogger logger)
        {
            _audioService = audioService;
            _messageCenterService = messageCenterService;
            _blobStorage = blobStorage;
            _diskStorage = index[StorageLocation.Chunk];
            _audioFileRepository = audioFileRepository;
            _mapper = mapper;
            _logger = logger.ForContext<UploadAudioFileCommand>();
        }

        protected override async Task<CommandResult<FileItemOutputModel>> Execute(UploadAudioFilePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                if (validationResult.Errors.ContainsError(nameof(UploadAudioFilePayload.File), ValidationErrorCodes.ParameterIsNull))
                {
                    _logger.Error($"[{userId}] Uploaded file source was not found");
                    throw new OperationErrorException(ErrorCode.EC100);
                }

                if (validationResult.Errors.ContainsError(nameof(UploadAudioFilePayload.Language), ValidationErrorCodes.NotSupportedLanguage))
                {
                    _logger.Error($"[{userId}] Language {parameter.Language} is not supported");
                    throw new OperationErrorException(ErrorCode.EC200);
                }

                if (validationResult.Errors.ContainsError(nameof(UploadAudioFilePayload.Language), ValidationErrorCodes.NotSupportedLanguageModel))
                {
                    _logger.Error($"[{userId}] Language phone call model is not supported");
                    throw new OperationErrorException(ErrorCode.EC203);
                }

                if (validationResult.Errors.ContainsError(nameof(UploadAudioFilePayload.File), ValidationErrorCodes.NotSupportedContentType))
                {
                    _logger.Error($"[{userId}] Audio file content type {parameter.File.ContentType} is not supported");
                    throw new OperationErrorException(ErrorCode.EC201);
                }

                if (validationResult.Errors.ContainsError(nameof(UploadAudioFilePayload.TranscriptionEndTime), ValidationErrorCodes.StartTimeGreaterOrEqualThanEndTime))
                {
                    _logger.Error($"[{userId}] Start time for transcription is greater or equal than end time");
                    throw new OperationErrorException(ErrorCode.EC204);
                }

                _logger.Error($"[{userId}] Invalid input data");
                throw new OperationErrorException(ErrorCode.EC600);
            }

            var audioFileId = Guid.NewGuid();
            var tempFilePath = string.Empty;
            var sourceName = string.Empty;
            var isOperationSuccessful = false;

            try
            {
                var uploadedFileSource = await parameter.File.GetBytesAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                tempFilePath = await _diskStorage.UploadAsync(uploadedFileSource, cancellationToken);
                _logger.Verbose($"[{userId}] Audio file was uploaded on temporary destination: {tempFilePath}");

                var audioFileTime = _audioService.GetTotalTime(tempFilePath);
                if (!audioFileTime.HasValue)
                {
                    _logger.Error($"[{userId}] Audio file content type {parameter.File.ContentType} is not supported");
                    throw new OperationErrorException(ErrorCode.EC201);
                }

                if (audioFileTime < parameter.TranscriptionEndTime)
                {
                    _logger.Error($"[{userId}] Transcription end time greater than total time of the audio file");
                    throw new OperationErrorException(ErrorCode.EC205);
                }

                cancellationToken.ThrowIfCancellationRequested();

                _logger.Verbose($"[{userId}] Start uploading audio file to blob storage");

                var contentType = parameter.File.ContentType;
                var uploadBlobSettings = new UploadBlobSettings(tempFilePath, userId, audioFileId, contentType);
                sourceName = await _blobStorage.UploadAsync(uploadBlobSettings, cancellationToken);

                _logger.Verbose($"[{userId}] Audio file source {sourceName} was uploaded to blob storage for audio file {audioFileId}");

                cancellationToken.ThrowIfCancellationRequested();

                var audioFile = new AudioFile
                {
                    Id = audioFileId,
                    UserId = userId,
                    ApplicationId = parameter.ApplicationId,
                    Name = parameter.Name,
                    FileName = parameter.FileName,
                    Language = parameter.Language,
                    IsPhoneCall = parameter.IsPhoneCall,
                    OriginalSourceFileName = sourceName,
                    UploadStatus = UploadStatus.Completed,
                    TotalTime = audioFileTime.Value,
                    TranscriptionStartTime = parameter.TranscriptionStartTime,
                    TranscriptionEndTime = parameter.TranscriptionEndTime,
                    DateCreated = parameter.DateCreated,
                    DateUpdatedUtc = DateTime.UtcNow
                };

                await _audioFileRepository.AddAsync(audioFile);
                await _audioFileRepository.SaveAsync(cancellationToken);
                await _messageCenterService.SendAsync(HubMethodsHelper.GetFilesListChangedMethod(userId));
                isOperationSuccessful = true;

                _logger.Information($"[{userId}] Audio file was successfully submitted. Audio file ID = {audioFile.Id}, name = {audioFile.Name}, file name = {audioFile.FileName}");

                var outputModel = _mapper.Map<FileItemOutputModel>(audioFile);
                return new CommandResult<FileItemOutputModel>(outputModel);
            }
            catch (RequestFailedException ex)
            {
                _logger.Error(ex, $"[{userId}] Blob storage is unavailable");

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

                    var deleteBlobSettings = new DeleteBlobSettings(sourceName, userId, audioFileId);
                    await _blobStorage.DeleteFileBlobAsync(deleteBlobSettings, default);
                }

                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);

                    _logger.Verbose($"[{userId}] Audio file was removed on destination: {tempFilePath}");
                }
            }
        }
    }
}
