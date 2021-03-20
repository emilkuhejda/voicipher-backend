using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using AutoMapper;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Business.Utils;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels.Audio;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Audio
{
    public class UpdateAudioFileCommand : Command<UpdateAudioFileInputModel, CommandResult<FileItemOutputModel>>, IUpdateAudioFileCommand
    {
        private readonly IAudioService _audioService;
        private readonly IMessageCenterService _messageCenterService;
        private readonly IFileAccessService _fileAccessService;
        private readonly IBlobStorage _blobStorage;
        private readonly IDiskStorage _diskStorage;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UpdateAudioFileCommand(
            IAudioService audioService,
            IMessageCenterService messageCenterService,
            IFileAccessService fileAccessService,
            IBlobStorage blobStorage,
            IIndex<StorageLocation, IDiskStorage> index,
            IAudioFileRepository audioFileRepository,
            IMapper mapper,
            ILogger logger)
        {
            _audioService = audioService;
            _messageCenterService = messageCenterService;
            _fileAccessService = fileAccessService;
            _blobStorage = blobStorage;
            _diskStorage = index[StorageLocation.Chunk];
            _audioFileRepository = audioFileRepository;
            _mapper = mapper;
            _logger = logger.ForContext<UpdateAudioFileCommand>();
        }

        protected override async Task<CommandResult<FileItemOutputModel>> Execute(UpdateAudioFileInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                if (validationResult.Errors.ContainsError(nameof(UpdateAudioFileInputModel.Language), ValidationErrorCodes.NotSupportedLanguage))
                {
                    _logger.Error($"[{userId}] Language {parameter.Language} is not supported");
                    throw new OperationErrorException(ErrorCode.EC200);
                }

                if (validationResult.Errors.ContainsError(nameof(UpdateAudioFileInputModel.Language), ValidationErrorCodes.NotSupportedLanguageModel))
                {
                    _logger.Error($"[{userId}] Language phone call model is not supported");
                    throw new OperationErrorException(ErrorCode.EC203);
                }

                if (validationResult.Errors.ContainsError(nameof(UpdateAudioFileInputModel.TranscriptionEndTime), ValidationErrorCodes.StartTimeGreaterOrEqualThanEndTime))
                {
                    _logger.Error($"[{userId}] Start time for transcription is greater or equal than end time");
                    throw new OperationErrorException(ErrorCode.EC204);
                }

                _logger.Error($"[{userId}] Invalid input data");
                throw new OperationErrorException(ErrorCode.EC600);
            }

            var audioFile = await _audioFileRepository.GetAsync(userId, parameter.AudioFileId, cancellationToken);
            if (audioFile == null)
            {
                _logger.Error($"[{userId}] Audio file {parameter.AudioFileId} was not found");
                throw new OperationErrorException(ErrorCode.EC101);
            }

            if (parameter.File != null)
            {
                var tempFilePath = string.Empty;
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var uploadedFileSource = await parameter.File.GetBytesAsync(cancellationToken);
                    tempFilePath = await _diskStorage.UploadAsync(uploadedFileSource, cancellationToken);
                    _logger.Verbose($"[{userId}] Audio file was uploaded on temporary destination: {tempFilePath}");

                    cancellationToken.ThrowIfCancellationRequested();

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

                    _logger.Verbose($"[{userId}] Start uploading audio file to blob storage");

                    var contentType = parameter.File.ContentType;
                    var uploadBlobSettings = new UploadBlobSettings(tempFilePath, userId, audioFile.Id, audioFile.OriginalSourceFileName, contentType);
                    var sourceName = await _blobStorage.UploadAsync(uploadBlobSettings, cancellationToken);

                    audioFile.OriginalSourceFileName = sourceName;
                    audioFile.FileName = parameter.FileName;

                    _logger.Verbose($"[{userId}] Audio file source {sourceName} was uploaded to blob storage for audio file {audioFile.Id}");

                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (OperationErrorException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"[{userId}] Audio file source update failed");
                }
                finally
                {
                    _fileAccessService.Delete(tempFilePath);
                }
            }
            else
            {
                if (audioFile.TotalTime < parameter.TranscriptionEndTime)
                {
                    _logger.Error($"[{userId}] Transcription end time greater than total time of the audio file");
                    throw new OperationErrorException(ErrorCode.EC205);
                }
            }

            audioFile.ApplicationId = parameter.ApplicationId;
            audioFile.Name = parameter.Name;
            audioFile.Language = parameter.Language;
            audioFile.IsPhoneCall = parameter.IsPhoneCall;
            audioFile.TranscriptionStartTime = parameter.TranscriptionStartTime;
            audioFile.TranscriptionEndTime = parameter.TranscriptionEndTime;
            audioFile.DateUpdatedUtc = DateTime.UtcNow;

            await _audioFileRepository.SaveAsync(cancellationToken);
            await _messageCenterService.SendAsync(HubMethodsHelper.GetFilesListChangedMethod(userId));

            var outputModel = _mapper.Map<FileItemOutputModel>(audioFile);
            return new CommandResult<FileItemOutputModel>(outputModel);
        }
    }
}
