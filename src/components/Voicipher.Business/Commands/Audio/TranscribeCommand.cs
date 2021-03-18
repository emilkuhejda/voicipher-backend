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
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads;
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Domain.Utils;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Audio
{
    public class TranscribeCommand : Command<TranscribePayload, CommandResult<OkOutputModel>>, ITranscribeCommand
    {
        private readonly ICanRunRecognitionCommand _canRunRecognitionCommand;
        private readonly IAudioFileProcessingChannel _audioFileProcessingChannel;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IBackgroundJobRepository _backgroundJobRepository;
        private readonly ILogger _logger;

        public TranscribeCommand(
            ICanRunRecognitionCommand canRunRecognitionCommand,
            IAudioFileProcessingChannel audioFileProcessingChannel,
            IAudioFileRepository audioFileRepository,
            IBackgroundJobRepository backgroundJobRepository,
            ILogger logger)
        {
            _canRunRecognitionCommand = canRunRecognitionCommand;
            _audioFileProcessingChannel = audioFileProcessingChannel;
            _audioFileRepository = audioFileRepository;
            _backgroundJobRepository = backgroundJobRepository;
            _logger = logger.ForContext<TranscribeCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(TranscribePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            _logger.Verbose($"[{userId}] Start validation before transcription. Audio file ID = {parameter.AudioFileId}");

            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                if (validationResult.Errors.ContainsError(nameof(TranscribePayload.Language), ValidationErrorCodes.NotSupportedLanguageModel))
                {
                    _logger.Error($"[{userId}] Language phone call model is not supported");
                    throw new OperationErrorException(ErrorCode.EC203);
                }

                if (validationResult.Errors.ContainsError(nameof(TranscribePayload.Language), ValidationErrorCodes.NotSupportedLanguage))
                {
                    _logger.Error($"[{userId}] Language {parameter.Language} is not supported");
                    throw new OperationErrorException(ErrorCode.EC200);
                }

                if (validationResult.Errors.ContainsError(nameof(TranscribePayload.EndTime), ValidationErrorCodes.StartTimeGreaterOrEqualThanEndTime))
                {
                    _logger.Error($"[{userId}] Start time for transcription is greater or equal than end time");
                    throw new OperationErrorException(ErrorCode.EC600);
                }

                _logger.Error($"[{userId}] Invalid input data");
                throw new OperationErrorException(ErrorCode.EC600);
            }

            if (_audioFileProcessingChannel.IsProcessingForUser(userId))
            {
                _logger.Error($"[{userId}] User try to run more then one file recognition");
                throw new OperationErrorException(ErrorCode.EC303);
            }

            var restartedAttempts = await _backgroundJobRepository.GetAttemptsCountAsync(parameter.AudioFileId, cancellationToken);
            if (restartedAttempts > 1)
            {
                _logger.Error($"[{userId}] Too many attempts ({restartedAttempts}) to restart has been done for audio file {parameter.AudioFileId}");
                throw new OperationErrorException(ErrorCode.EC304);
            }

            var audioFile = await _audioFileRepository.GetAsync(userId, parameter.AudioFileId, cancellationToken);
            if (audioFile == null)
            {
                _logger.Error($"[{userId}] Audio file {parameter.AudioFileId} not exists");
                throw new OperationErrorException(ErrorCode.EC101);
            }

            if (audioFile.IsPhoneCall && !SupportedLanguages.IsPhoneCallModelSupported(parameter.Language))
            {
                _logger.Error($"[{userId}] Language phone call model is not supported");
                throw new OperationErrorException(ErrorCode.EC203);
            }

            if (audioFile.UploadStatus != UploadStatus.Completed)
            {
                _logger.Error($"[{userId}] Audio file source {parameter.AudioFileId} is not uploaded. Uploaded state is {audioFile.UploadStatus}");
                throw new OperationErrorException(ErrorCode.EC104);
            }

            if (audioFile.RecognitionState != RecognitionState.None)
            {
                _logger.Error($"[{userId}] Audio file {parameter.AudioFileId} is in the wrong recognition state. Recognition state is {audioFile.RecognitionState}");
                throw new OperationErrorException(ErrorCode.EC103);
            }

            var canRunRecognitionPayload = new CanRunRecognitionPayload(userId);
            var canRunRecognitionResult = await _canRunRecognitionCommand.ExecuteAsync(canRunRecognitionPayload, principal, cancellationToken);
            if (!canRunRecognitionResult.IsSuccess)
            {
                _logger.Error($"[{userId}] User has no enough left minutes in subscription. Command finished with error code {canRunRecognitionResult.Error.ErrorCode}");
                throw new OperationErrorException(ErrorCode.EC300);
            }

            audioFile.Language = parameter.Language;
            audioFile.IsPhoneCall = parameter.IsPhoneCall;
            audioFile.TranscriptionStartTime = parameter.StartTime;
            audioFile.TranscriptionEndTime = parameter.EndTime == TimeSpan.Zero ? audioFile.TotalTime : parameter.EndTime;
            audioFile.ApplicationId = parameter.ApplicationId;
            audioFile.DateUpdatedUtc = DateTime.UtcNow;
            await _audioFileRepository.SaveAsync(cancellationToken);

            _logger.Verbose($"[{userId}] Audio file {parameter.AudioFileId} has updated language to {parameter.Language}");

            await _audioFileProcessingChannel.AddFileAsync(new RecognitionFile(audioFile));

            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
