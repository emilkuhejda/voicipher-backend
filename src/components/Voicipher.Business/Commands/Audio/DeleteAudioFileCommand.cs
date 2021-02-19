using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
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
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads.Audio;

namespace Voicipher.Business.Commands.Audio
{
    public class DeleteAudioFileCommand : Command<DeleteAudioFilePayload, CommandResult<OkOutputModel>>, IDeleteAudioFileCommand
    {
        private readonly IMessageCenterService _messageCenterService;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly ILogger _logger;

        public DeleteAudioFileCommand(
            IMessageCenterService messageCenterService,
            IAudioFileRepository audioFileRepository,
            ILogger logger)
        {
            _messageCenterService = messageCenterService;
            _audioFileRepository = audioFileRepository;
            _logger = logger.ForContext<DeleteAudioFileCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(DeleteAudioFilePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            if (!parameter.Validate().IsValid)
            {
                _logger.Error($"[{userId}] Invalid input data");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            var audioFile = await _audioFileRepository.GetAsync(userId, parameter.AudioFileId, cancellationToken);
            if (audioFile == null)
            {
                _logger.Error($"[{userId}] Audio file {parameter.AudioFileId} not found");

                throw new OperationErrorException(ErrorCode.EC101);
            }

            if (audioFile.UploadStatus != UploadStatus.Completed)
            {
                _logger.Error($"[{userId}] Audio file source {parameter.AudioFileId} is not uploaded. Uploaded state is {audioFile.UploadStatus}");

                throw new OperationErrorException(ErrorCode.EC104);
            }

            audioFile.ApplicationId = parameter.ApplicationId;
            audioFile.DateUpdatedUtc = DateTime.UtcNow;
            audioFile.IsDeleted = true;
            await _audioFileRepository.SaveAsync(cancellationToken);

            await _messageCenterService.SendAsync(HubMethodsHelper.GetFilesListChangedMethod(userId));

            _logger.Information($"[{userId}] Audio file {parameter.AudioFileId} was deleted");

            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
