using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Business.Utils;
using Voicipher.DataAccess;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Payloads.Transcription;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Transcription
{
    public class UpdateRecognitionStateCommand : Command<UpdateRecognitionStatePayload, CommandResult>, IUpdateRecognitionStateCommand
    {
        private readonly IMessageCenterService _messageCenterService;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public UpdateRecognitionStateCommand(
            IMessageCenterService messageCenterService,
            IAudioFileRepository audioFileRepository,
            IUnitOfWork unitOfWork,
            ILogger logger)
        {
            _messageCenterService = messageCenterService;
            _audioFileRepository = audioFileRepository;
            _unitOfWork = unitOfWork;
            _logger = logger.ForContext<UpdateRecognitionStateCommand>();
        }

        protected override async Task<CommandResult> Execute(UpdateRecognitionStatePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var audioFile = await _audioFileRepository.GetAsync(parameter.UserId, parameter.AudioFileId, cancellationToken);
            if (audioFile == null)
                return new CommandResult(new OperationError(ValidationErrorCodes.NotFound));

            var oldRecognitionState = audioFile.RecognitionState;
            audioFile.RecognitionState = parameter.RecognitionState;
            audioFile.ApplicationId = parameter.ApplicationId;
            audioFile.DateUpdatedUtc = DateTime.UtcNow;

            await _unitOfWork.SaveAsync(cancellationToken);

            _logger.Information($"[{parameter.UserId}] Audio file {parameter.AudioFileId} changed recognition state from {oldRecognitionState} to {parameter.RecognitionState}");

            await _messageCenterService.SendAsync(
                HubMethodsHelper.GetRecognitionStateChangedMethod(parameter.UserId),
                parameter.AudioFileId,
                parameter.RecognitionState.ToString());

            return new CommandResult();
        }
    }
}
