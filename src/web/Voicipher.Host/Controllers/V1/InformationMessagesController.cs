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
        private readonly Lazy<IInformationMessageRepository> _informationMessageRepository;
        private readonly Lazy<IMapper> _mapper;

        public InformationMessagesController(
            Lazy<IInformationMessageRepository> informationMessageRepository,
            Lazy<IMapper> mapper)
        {
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
        // [ProducesResponseType(typeof(InformationMessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Get(Guid informationMessageId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("mark-as-opened")]
        // [ProducesResponseType(typeof(InformationMessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "MarkMessageAsOpened")]
        public IActionResult MarkAsOpened(Guid informationMessageId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("mark-messages-as-opened")]
        // [ProducesResponseType(typeof(OkDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "MarkMessagesAsOpened")]
        public IActionResult MarkMessagesAsOpened(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }
    }
}
