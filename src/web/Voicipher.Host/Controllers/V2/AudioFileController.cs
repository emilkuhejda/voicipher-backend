using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Voicipher.Host.Controllers.V2
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioFileController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(string.Empty);
        }
    }
}
