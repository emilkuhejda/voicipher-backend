using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
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
    public class PermanentDeleteAllCommand : Command<PermanentDeleteAllPayload, CommandResult<OkOutputModel>>, IPermanentDeleteAllCommand
    {
        private readonly IMessageCenterService _messageCenterService;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IBlobStorage _blobStorage;
        private readonly ILogger _logger;

        public PermanentDeleteAllCommand(
            IMessageCenterService messageCenterService,
            IAudioFileRepository audioFileRepository,
            IBlobStorage blobStorage,
            ILogger logger)
        {
            _messageCenterService = messageCenterService;
            _audioFileRepository = audioFileRepository;
            _blobStorage = blobStorage;
            _logger = logger;
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(PermanentDeleteAllPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (!parameter.Validate().IsValid)
            {
                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            var userId = principal.GetNameIdentifier();

            await Task.CompletedTask;
            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
