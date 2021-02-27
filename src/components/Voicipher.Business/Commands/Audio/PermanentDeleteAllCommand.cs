using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Business.Utils;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads.Audio;

namespace Voicipher.Business.Commands.Audio
{
    public class PermanentDeleteAllCommand : Command<PermanentDeleteAllPayload, CommandResult<OkOutputModel>>, IPermanentDeleteAllCommand
    {
        private readonly IMessageCenterService _messageCenterService;
        private readonly IBlobStorage _blobStorage;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public PermanentDeleteAllCommand(
            IMessageCenterService messageCenterService,
            IBlobStorage blobStorage,
            IAudioFileRepository audioFileRepository,
            IUnitOfWork unitOfWork,
            ILogger logger)
        {
            _messageCenterService = messageCenterService;
            _blobStorage = blobStorage;
            _audioFileRepository = audioFileRepository;
            _unitOfWork = unitOfWork;
            _logger = logger.ForContext<PermanentDeleteAllCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(PermanentDeleteAllPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (!parameter.Validate().IsValid)
            {
                _logger.Error($"[{parameter.UserId}] Invalid input data");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            try
            {
                var audioFilesToDelete = await _audioFileRepository.GetForPermanentDeleteAllAsync(parameter.UserId, parameter.AudioFilesIds, parameter.ApplicationId, cancellationToken);
                foreach (var audioFile in audioFilesToDelete)
                {
                    _logger.Verbose($"[{parameter.UserId}] Start deleting audio file {audioFile.Id}");
                    await _blobStorage.DeleteAudioFileAsync(new BlobSettings(audioFile.Id, parameter.UserId), cancellationToken);
                    _logger.Verbose($"[{parameter.UserId}] Delete audio file {audioFile.Id} from blob storage");

                    var deletedEntity = audioFile.CreateDeletedEntity(parameter.ApplicationId);
                    _audioFileRepository.Remove(audioFile);
                    await _audioFileRepository.AddAsync(deletedEntity);
                }

                await _unitOfWork.SaveAsync(cancellationToken);

                await _messageCenterService.SendAsync(HubMethodsHelper.GetFilesListChangedMethod(parameter.UserId));

                _logger.Information($"[{parameter.UserId}] Audio files {JsonConvert.SerializeObject(parameter.AudioFilesIds)} were permanently deleted");

                return new CommandResult<OkOutputModel>(new OkOutputModel());
            }
            catch (RequestFailedException ex)
            {
                _logger.Error(ex, $"[{parameter.UserId}] Blob storage is unavailable. Audio files = {JsonConvert.SerializeObject(parameter.AudioFilesIds)}");

                throw new OperationErrorException(ErrorCode.EC700);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, $"[{parameter.UserId}] Permanent audio files deletion failed");

                throw new OperationErrorException(ErrorCode.EC603);
            }
        }
    }
}
