using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Business.Utils;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Domain.Settings;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Audio
{
    public class DeleteAudioFileSourceCommand : Command<DeleteAudioFileSourcePayload, CommandResult>, IDeleteAudioFileSourceCommand
    {
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IMessageCenterService _messageCenterService;
        private readonly IBlobStorage _blobStorage;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public DeleteAudioFileSourceCommand(
            IAudioFileRepository audioFileRepository,
            IMessageCenterService messageCenterService,
            IBlobStorage blobStorage,
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _audioFileRepository = audioFileRepository;
            _messageCenterService = messageCenterService;
            _blobStorage = blobStorage;
            _appSettings = options.Value;
            _logger = logger.ForContext<DeleteAudioFileSourceCommand>();
        }

        protected override async Task<CommandResult> Execute(DeleteAudioFileSourcePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var audioFile = await _audioFileRepository.GetAsync(parameter.UserId, parameter.AudioFileId, cancellationToken);
            if (audioFile == null)
            {
                _logger.Error($"[{parameter.UserId}] Audio file {parameter.AudioFileId} not found");

                return new CommandResult(new OperationError(ValidationErrorCodes.NotFound));
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(audioFile.OriginalSourceFileName))
                {
                    var deleteBlobSettings = new DeleteBlobSettings(audioFile.OriginalSourceFileName, audioFile.UserId, audioFile.Id);
                    await _blobStorage.DeleteFileBlobAsync(deleteBlobSettings, cancellationToken);

                    _logger.Information($"[{parameter.UserId}] Original audio file source {audioFile.OriginalSourceFileName} was deleted from storage");
                }

                if (!string.IsNullOrWhiteSpace(audioFile.SourceFileName))
                {
                    var deleteBlobSettings = new DeleteBlobSettings(audioFile.SourceFileName, audioFile.UserId, audioFile.Id);
                    await _blobStorage.DeleteFileBlobAsync(deleteBlobSettings, cancellationToken);

                    _logger.Information($"[{parameter.UserId}] Audio file source {audioFile.SourceFileName} was deleted from storage");
                }

                audioFile.OriginalSourceFileName = string.Empty;
                audioFile.SourceFileName = string.Empty;
                audioFile.ApplicationId = _appSettings.ApplicationId;
                audioFile.DateUpdatedUtc = DateTime.UtcNow;

                await _audioFileRepository.SaveAsync(cancellationToken);

                _logger.Information($"[{parameter.UserId}] Audio file {audioFile.Id} source was updated");

                await _messageCenterService.SendAsync(HubMethodsHelper.GetFilesListChangedMethod(audioFile.UserId));

                return new CommandResult();
            }
            catch (RequestFailedException ex)
            {
                _logger.Error(ex, $"[{parameter.UserId}] Blob storage is unavailable. Audio file ID = {parameter.AudioFileId}");

                return new CommandResult(new OperationError(ValidationErrorCodes.UnavailableBlobStorage));
            }
        }
    }
}
