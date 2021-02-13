using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/contact-form")]
    [Produces("application/json")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class ContactFormController : ControllerBase
    {
        [HttpPost("create")]
        // [ProducesResponseType(typeof(OkDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "CreateContactForm")]
        public IActionResult Create([FromBody] object contactFormModel)
        {
            throw new NotImplementedException();
        }
    }
}
