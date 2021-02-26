using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/clean-up")]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    [AllowAnonymous]
    public class CleanUpController : ControllerBase
    {
        [HttpPost("audio-files")]
        public async Task<IActionResult> CleanUpAudioFiles()
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpPost("transcribe-item")]
        public async Task<IActionResult> CleanUpTranscribeItems()
        {
            await Task.CompletedTask;
            return Ok();
        }
    }
}
