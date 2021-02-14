using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
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
    public class DeleteAllAudioFileCommand : Command<DeleteAllAudioFilePayload, CommandResult<OkOutputModel>>, IDeleteAllAudioFileCommand
    {
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IMessageCenterService _messageCenterService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DeleteAllAudioFileCommand(
            IAudioFileRepository audioFileRepository,
            IMessageCenterService messageCenterService,
            IMapper mapper,
            ILogger logger)
        {
            _audioFileRepository = audioFileRepository;
            _messageCenterService = messageCenterService;
            _mapper = mapper;
            _logger = logger;
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(DeleteAllAudioFilePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (!parameter.Validate().IsValid)
            {
                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            var userId = principal.GetNameIdentifier();
            var audioFileIds = parameter.AudioFiles.Select(x => x.Id).ToArray();
            var audioFiles = await _audioFileRepository.GetForDeleteAllAsync(userId, audioFileIds, parameter.ApplicationId, cancellationToken);

            foreach (var audioFile in audioFiles)
            {
                var deletedAudioFile = parameter.AudioFiles.Single(x => x.Id == audioFile.Id);
                if (deletedAudioFile.DeletedDate < audioFile.DateUpdatedUtc)
                    continue;

                audioFile.ApplicationId = parameter.ApplicationId;
                audioFile.DateUpdatedUtc = DateTime.UtcNow;
                audioFile.IsDeleted = true;
            }

            await _audioFileRepository.SaveAsync(cancellationToken);

            await _messageCenterService.SendAsync(HubMethodsHelper.GetFilesListChangedMethod(userId));

            var audioFilesIds = audioFiles.Select(x => x.Id).ToList();
            _logger.Information($"File items '{JsonConvert.SerializeObject(audioFilesIds)}' were deleted.");

            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
