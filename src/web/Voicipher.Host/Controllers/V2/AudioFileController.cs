using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;

namespace Voicipher.Host.Controllers.V2
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioFileController : ControllerBase
    {
        private readonly ILogger _logger;

        public AudioFileController(ILogger logger)
        {
            _logger = logger.ForContext<AudioFileController>();
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = "AudioFiles")]
        public IActionResult Get()
        {
            _logger.Information("This is INFO.");

            return Ok(string.Empty);
        }
    }
}
