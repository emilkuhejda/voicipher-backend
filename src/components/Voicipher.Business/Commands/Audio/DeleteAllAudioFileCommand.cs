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
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads.Audio;

namespace Voicipher.Business.Commands.Audio
{
    public class DeleteAllAudioFileCommand : Command<DeletedAudioFilePayload, CommandResult<OkOutputModel>>, IDeleteAllAudioFileCommand
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

        protected override async Task<CommandResult<OkOutputModel>> Execute(DeletedAudioFilePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (!parameter.Validate().IsValid)
            {
                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            var userId = principal.GetNameIdentifier();
            var audioFiles = parameter.AudioFiles.Select(x => _mapper.Map<DeletedAudioFile>(x)).ToArray();

            await _audioFileRepository.DeleteAllAsync(userId, audioFiles, parameter.ApplicationId, cancellationToken);
            await _audioFileRepository.SaveAsync(cancellationToken);

            await _messageCenterService.SendAsync(HubMethodsHelper.GetFilesListChangedMethod(userId));

            var audioFilesIds = audioFiles.Select(x => x.Id).ToList();
            _logger.Information($"File items '{JsonConvert.SerializeObject(audioFilesIds)}' were deleted.");

            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
