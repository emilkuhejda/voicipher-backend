using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
        [HttpGet]
        // [ProducesResponseType(typeof(IEnumerable<InformationMessageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetInformationMessages")]
        public IActionResult GetAll(DateTime updatedAfter)
        {
            throw new NotImplementedException();
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
