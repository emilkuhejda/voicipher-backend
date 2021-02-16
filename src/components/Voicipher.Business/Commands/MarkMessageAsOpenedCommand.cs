using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Commands
{
    public class MarkMessageAsOpenedCommand : Command<Guid[], CommandResult<InformationMessageOutputModel[]>>, IMarkMessageAsOpenedCommand
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

        protected override async Task<CommandResult<InformationMessageOutputModel[]>> Execute(Guid[] parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            var informationMessages = await _informationMessageRepository.GetByUserIdAsync(userId, parameter, cancellationToken);

            foreach (var informationMessage in informationMessages)
            {
                informationMessage.DateUpdatedUtc = DateTime.UtcNow;
                informationMessage.WasOpened = true;
            }

            await _informationMessageRepository.SaveAsync(cancellationToken);

            _logger.Information($"Information messages '{parameter}' were mark as opened");

            var outputModel = informationMessages.Select(_mapper.Map<InformationMessageOutputModel>).ToArray();
            return new CommandResult<InformationMessageOutputModel[]>(outputModel);
        }
    }
}
