using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Voicipher.Host.Controllers.V2
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioFileController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = "AudioFiles")]
        public IActionResult Get()
        {
            return Ok(string.Empty);
        }
    }
}
