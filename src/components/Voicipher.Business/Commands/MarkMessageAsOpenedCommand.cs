using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Commands
{
    public class MarkMessageAsOpenedCommand : Command<Guid, CommandResult<InformationMessageOutputModel>>, IMarkMessageAsOpenedCommand
    {
        private readonly IInformationMessageRepository _informationMessageRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public MarkMessageAsOpenedCommand(
            IInformationMessageRepository informationMessageRepository,
            IMapper mapper,
            ILogger logger)
        {
            _informationMessageRepository = informationMessageRepository;
            _mapper = mapper;
            _logger = logger.ForContext<MarkMessageAsOpenedCommand>();
        }

        protected override async Task<CommandResult<InformationMessageOutputModel>> Execute(Guid parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            var informationMessage = await _informationMessageRepository.GetByUserIdAsync(userId, parameter, cancellationToken);
            if (informationMessage == null)
            {
                _logger.Error($"Information message '{parameter}' not found.");

                throw new OperationErrorException();
            }

            informationMessage.DateUpdatedUtc = DateTime.UtcNow;
            informationMessage.WasOpened = true;

            await _informationMessageRepository.SaveAsync(cancellationToken);

            _logger.Information($"Information message '{informationMessage.Id}' was mark as opened.");

            var outputModel = _mapper.Map<InformationMessageOutputModel>(informationMessage);
            return new CommandResult<InformationMessageOutputModel>(outputModel);
        }
    }
}
