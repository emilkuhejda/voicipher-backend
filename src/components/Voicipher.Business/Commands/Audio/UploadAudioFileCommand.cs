using System;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Audio
{
    public class UploadAudioFileCommand : Command<UploadAudioFilePayload, CommandResult<FileItemOutputModel>>, IUploadAudioFileCommand
    {
        private readonly IAudioService _audioService;
        private readonly IBlobStorage _blobStorage;
        private readonly IChunkStorage _chunkStorage;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UploadAudioFileCommand(
            IAudioService audioService,
            IBlobStorage blobStorage,
            IChunkStorage chunkStorage,
            IAudioFileRepository audioFileRepository,
            IMapper mapper,
            ILogger logger)
        {
            _audioService = audioService;
            _blobStorage = blobStorage;
            _chunkStorage = chunkStorage;
            _audioFileRepository = audioFileRepository;
            _mapper = mapper;
            _logger = logger.ForContext<UploadAudioFileCommand>();
        }

        protected override async Task<CommandResult<FileItemOutputModel>> Execute(UploadAudioFilePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                if (validationResult.Errors.ContainsError(nameof(UploadAudioFilePayload.File), ValidationErrorCodes.ParameterIsNull))
                {
                    _logger.Error("Uploaded file source was not found.");

                    throw new OperationErrorException(ErrorCode.EC100);
                }

                if (validationResult.Errors.ContainsError(nameof(UploadAudioFilePayload.Language), ValidationErrorCodes.NotSupportedLanguage))
                {
                    _logger.Error($"Language '{parameter.Language}' is not supported.");

                    throw new OperationErrorException(ErrorCode.EC200);
                }

                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            var userId = principal.GetNameIdentifier();
            var audioFileId = Guid.NewGuid();
            var tempFilePath = string.Empty;

            try
            {
                var uploadedFileSource = await parameter.File.GetBytesAsync(cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();

                tempFilePath = await _chunkStorage.UploadAsync(uploadedFileSource, cancellationToken);
                _logger.Information($"Audio file was uploaded on temporary destination: '{tempFilePath}'.");

                var audioFileTime = _audioService.GetTotalTime(tempFilePath);
                if (!audioFileTime.HasValue)
                {
                    _logger.Error($"Audio file '{parameter.FileName}' is not supported. [{userId}]");

                    throw new OperationErrorException(ErrorCode.EC201);
                }

                cancellationToken.ThrowIfCancellationRequested();

                var uploadBlobSettings = new UploadBlobSettings(userId, audioFileId, tempFilePath);
                var sourceName = await _blobStorage.UploadAsync(uploadBlobSettings);
                _logger.Information($"Audio file '{sourceName}' was uploaded to blob storage. Audio file ID = {audioFileId}. [{userId}]");

                cancellationToken.ThrowIfCancellationRequested();

                var audioFile = new AudioFile
                {
                    Id = audioFileId,
                    UserId = userId,
                    ApplicationId = parameter.ApplicationId,
                    Name = parameter.Name,
                    FileName = parameter.FileName,
                    Language = parameter.Language,
                    OriginalSourceFileName = parameter.FileName,
                    Storage = StorageSetting.Azure,
                    UploadStatus = UploadStatus.Completed,
                    TotalTime = audioFileTime.Value,
                    DateCreated = parameter.DateCreated,
                    DateUpdatedUtc = DateTime.UtcNow
                };

                await _audioFileRepository.AddAsync(audioFile);
                await _audioFileRepository.SaveAsync(cancellationToken);
                _logger.Information($"Audio file '{audioFile.Id}' was successfully submitted. [{userId}]");

                var outputModel = _mapper.Map<FileItemOutputModel>(audioFile);
                return new CommandResult<FileItemOutputModel>(outputModel);
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
            }
        }
    }
}
