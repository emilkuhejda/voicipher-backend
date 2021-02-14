using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
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
    public class RestoreAllCommand : Command<RestoreAllPayload, CommandResult<OkOutputModel>>, IRestoreAllCommand
    {
        private readonly IMessageCenterService _messageCenterService;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly ILogger _logger;

        public RestoreAllCommand(
            IMessageCenterService messageCenterService,
            IAudioFileRepository audioFileRepository,
            ILogger logger)
        {
            _messageCenterService = messageCenterService;
            _audioFileRepository = audioFileRepository;
            _logger = logger.ForContext<RestoreAllCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(RestoreAllPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (!parameter.Validate().IsValid)
            {
                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            var userId = principal.GetNameIdentifier();
            var audioFiles = await _audioFileRepository.GetForRestoreAsync(userId, parameter.AudioFilesIds.ToArray(), parameter.ApplicationId, cancellationToken);

            foreach (var audioFile in audioFiles)
            {
                audioFile.ApplicationId = parameter.ApplicationId;
                audioFile.DateUpdatedUtc = DateTime.UtcNow;
                audioFile.IsDeleted = false;
            }

            await _audioFileRepository.SaveAsync(cancellationToken);
            await _messageCenterService.SendAsync(HubMethodsHelper.GetFilesListChangedMethod(userId));

            _logger.Information($"Audio files '{JsonConvert.SerializeObject(parameter.AudioFilesIds)}' were restored.");

            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
