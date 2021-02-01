using Microsoft.AspNetCore.Mvc;

namespace Voicipher.Host.Controllers.V2
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioFileController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
