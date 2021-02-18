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
        private readonly IDiskStorage _diskStorage;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UploadAudioFileCommand(
            IAudioService audioService,
            IBlobStorage blobStorage,
            IIndex<StorageLocation, IDiskStorage> index,
            IAudioFileRepository audioFileRepository,
            IMapper mapper,
            ILogger logger)
        {
            _audioService = audioService;
            _blobStorage = blobStorage;
            _diskStorage = index[StorageLocation.Chunk];
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
                var uploadedFileSource = await parameter.File.GetBytesAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                tempFilePath = await _diskStorage.UploadAsync(uploadedFileSource, cancellationToken);
                _logger.Information($"Audio file was uploaded on temporary destination: {tempFilePath}");

                var audioFileTime = _audioService.GetTotalTime(tempFilePath);
                if (!audioFileTime.HasValue)
                {
                    _logger.Error($"Audio file {parameter.FileName} is not supported");

                    throw new OperationErrorException(ErrorCode.EC201);
                }

                cancellationToken.ThrowIfCancellationRequested();

                var uploadBlobSettings = new UploadBlobSettings(tempFilePath, userId, audioFileId);
                var sourceName = await _blobStorage.UploadAsync(uploadBlobSettings, cancellationToken);
                _logger.Information($"Audio file {sourceName} was uploaded to blob storage. Audio file ID = {audioFileId}");

                cancellationToken.ThrowIfCancellationRequested();

                var audioFile = new AudioFile
                {
                    Id = audioFileId,
                    UserId = userId,
                    ApplicationId = parameter.ApplicationId,
                    Name = parameter.Name,
                    FileName = parameter.FileName,
                    Language = parameter.Language,
                    OriginalSourceFileName = sourceName,
                    Storage = StorageSetting.Azure,
                    UploadStatus = UploadStatus.Completed,
                    TotalTime = audioFileTime.Value,
                    DateCreated = parameter.DateCreated,
                    DateUpdatedUtc = DateTime.UtcNow
                };

                await _audioFileRepository.AddAsync(audioFile);
                await _audioFileRepository.SaveAsync(cancellationToken);
                _logger.Information($"Audio file was successfully submitted. Audio file ID = {audioFile.Id}, name = {audioFile.Name}, file name = {audioFile.FileName}, user ID = {userId}");

                var outputModel = _mapper.Map<FileItemOutputModel>(audioFile);
                return new CommandResult<FileItemOutputModel>(outputModel);
            }
            catch (RequestFailedException ex)
            {
                _logger.Error(ex, "Blob storage is unavailable");

                throw new OperationErrorException(ErrorCode.EC700);
            }
            catch (DbUpdateException ex)
            {
                _logger.Error($"Exception occurred during submitting file chunks. Message: {ex.Message}");
                _logger.Error(ExceptionFormatter.FormatException(ex));

                throw new OperationErrorException(ErrorCode.EC400);
            }
            catch (OperationCanceledException)
            {
                _logger.Information("Operation was cancelled");

                throw new OperationErrorException(ErrorCode.EC800);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);

                    _logger.Information($"Audio file was removed on destination: {tempFilePath}");
                }
            }
        }
    }
}
