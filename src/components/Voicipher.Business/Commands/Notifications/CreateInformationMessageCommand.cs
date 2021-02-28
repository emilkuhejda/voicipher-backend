using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Notifications;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads.Notifications;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Notifications
{
    public class CreateInformationMessageCommand : Command<InformationMessagePayload, CommandResult<InformationMessageOutputModel>>, ICreateInformationMessageCommand
    {
        private readonly IInformationMessageRepository _informationMessageRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CreateInformationMessageCommand(
            IInformationMessageRepository informationMessageRepository,
            IMapper mapper,
            ILogger logger)
        {
            _informationMessageRepository = informationMessageRepository;
            _mapper = mapper;
            _logger = logger.ForContext<CreateInformationMessageCommand>();
        }

        protected override async Task<CommandResult<InformationMessageOutputModel>> Execute(InformationMessagePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var informationMessage = _mapper.Map<InformationMessage>(parameter);
            if (!informationMessage.Validate().IsValid)
            {
                _logger.Error($"Information message {JsonConvert.SerializeObject(informationMessage)} is not valid");
                return new CommandResult<InformationMessageOutputModel>(new OperationError(ValidationErrorCodes.InvalidInputData));
            }

            await _informationMessageRepository.AddAsync(informationMessage);
            await _informationMessageRepository.SaveAsync(cancellationToken);

            _logger.Verbose($"Information message {informationMessage.Id} was created");

            var outputModel = _mapper.Map<InformationMessageOutputModel>(informationMessage);
            return new CommandResult<InformationMessageOutputModel>(outputModel);
        }
    }
}
