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
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Audio
{
    public class TranscribeCommand : Command<TranscribePayload, CommandResult<OkOutputModel>>, ITranscribeCommand
    {
        private readonly ICanRunRecognitionCommand _canRunRecognitionCommand;
        private readonly IAudioFileProcessingChannel _audioFileProcessingChannel;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly ILogger _logger;

        public TranscribeCommand(
            ICanRunRecognitionCommand canRunRecognitionCommand,
            IAudioFileProcessingChannel audioFileProcessingChannel,
            IAudioFileRepository audioFileRepository,
            ILogger logger)
        {
            _canRunRecognitionCommand = canRunRecognitionCommand;
            _audioFileProcessingChannel = audioFileProcessingChannel;
            _audioFileRepository = audioFileRepository;
            _logger = logger.ForContext<TranscribeCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(TranscribePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            _logger.Information($"Start validation before transcription. User ID = {userId}, Audio file ID = {parameter.AudioFileId}");

            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                if (validationResult.Errors.ContainsError(nameof(TranscribePayload.Language), ValidationErrorCodes.NotSupportedLanguage))
                {
                    _logger.Error($"Language {parameter.Language} is not supported.");
                    throw new OperationErrorException(ErrorCode.EC200);
                }

                _logger.Error("Invalid input data.");
                throw new OperationErrorException(ErrorCode.EC600);
            }

            var audioFile = await _audioFileRepository.GetAsync(userId, parameter.AudioFileId, cancellationToken);
            if (audioFile == null)
            {
                _logger.Error($"Audio file {parameter.AudioFileId} not exists");
                throw new OperationErrorException(ErrorCode.EC101);
            }

            if (audioFile.UploadStatus != UploadStatus.Completed)
            {
                _logger.Error($"Audio file source {parameter.AudioFileId} is not uploaded. Uploaded state is {audioFile.UploadStatus}");
                throw new OperationErrorException(ErrorCode.EC104);
            }

            if (audioFile.RecognitionState != RecognitionState.None)
            {
                _logger.Error($"Audio file {parameter.AudioFileId} is in the wrong recognition state. Recognition state is {audioFile.RecognitionState}.");
                throw new OperationErrorException(ErrorCode.EC103);
            }

            var canRunRecognitionPayload = new CanRunRecognitionPayload(userId);
            var canRunRecognitionResult = await _canRunRecognitionCommand.ExecuteAsync(canRunRecognitionPayload, principal, cancellationToken);
            if (!canRunRecognitionResult.IsSuccess)
            {
                _logger.Error($"User '{userId}' has no enough left minutes in subscription. Command finished with error code {canRunRecognitionResult.Error.ErrorCode}");
                throw new OperationErrorException(ErrorCode.EC300);
            }

            audioFile.ApplicationId = parameter.ApplicationId;
            audioFile.Language = parameter.Language;
            audioFile.DateUpdatedUtc = DateTime.UtcNow;
            await _audioFileRepository.SaveAsync(cancellationToken);

            _logger.Information($"Audio file {parameter.AudioFileId} has updated language to {parameter.Language}");

            await _audioFileProcessingChannel.AddFileAsync(new RecognitionFile(userId, audioFile.Id));

            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
