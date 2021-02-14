using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Business.Extensions;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/information-messages")]
    [Authorize(Policy = nameof(VoicipherPolicy.User))]
    [ApiController]
    public class InformationMessagesController : ControllerBase
    {
        private readonly Lazy<IMarkMessageAsOpenedCommand> _markMessageAsOpenedCommand;
        private readonly Lazy<IInformationMessageRepository> _informationMessageRepository;
        private readonly Lazy<IMapper> _mapper;

        public InformationMessagesController(
            Lazy<IMarkMessageAsOpenedCommand> markMessageAsOpenedCommand,
            Lazy<IInformationMessageRepository> informationMessageRepository,
            Lazy<IMapper> mapper)
        {
            _markMessageAsOpenedCommand = markMessageAsOpenedCommand;
            _informationMessageRepository = informationMessageRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InformationMessageOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetInformationMessages")]
        public async Task<IActionResult> GetAll(DateTime updatedAfter, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetNameIdentifier();
            var informationMessages = await _informationMessageRepository.Value.GetAllAsync(userId, updatedAfter, cancellationToken);

            var outputModels = informationMessages.Select(x => _mapper.Value.Map<InformationMessageOutputModel>(x));
            return Ok(outputModels);
        }

        [HttpGet("{informationMessageId}")]
        [ProducesResponseType(typeof(InformationMessageOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Get(Guid informationMessageId, CancellationToken cancellationToken)
        {
            var informationMessage = await _informationMessageRepository.Value.GetAsync(informationMessageId, cancellationToken);
            var outputModel = _mapper.Value.Map<InformationMessageOutputModel>(informationMessage);

            return Ok(outputModel);
        }

        [HttpPut("mark-as-opened")]
        [ProducesResponseType(typeof(InformationMessageOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "MarkMessageAsOpened")]
        public async Task<IActionResult> MarkAsOpened(Guid informationMessageId, CancellationToken cancellationToken)
        {
            var commandResult = await _markMessageAsOpenedCommand.Value.ExecuteAsync(informationMessageId, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpPut("mark-messages-as-opened")]
        [ProducesResponseType(typeof(OkOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "MarkMessagesAsOpened")]
        public async Task<IActionResult> MarkMessagesAsOpened(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }
    }
}
