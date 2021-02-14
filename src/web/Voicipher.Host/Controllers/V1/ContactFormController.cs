using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/contact-form")]
    [Produces("application/json")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class ContactFormController : ControllerBase
    {
        private readonly Lazy<IContactFormCommand> _contactFormCommand;

        public ContactFormController(Lazy<IContactFormCommand> contactFormCommand)
        {
            _contactFormCommand = contactFormCommand;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(OkOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "CreateContactForm")]
        public async Task<IActionResult> Create([FromBody] ContactFormInputModel contactFormInputModel, CancellationToken cancellationToken)
        {
            var commandResult = await _contactFormCommand.Value.ExecuteAsync(contactFormInputModel, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }
    }
}
