using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/files")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class FileItemController : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAll(Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("detail/{fileItemId}")]
        public async Task<IActionResult> Get(Guid fileItemId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("restore")]
        public async Task<IActionResult> Restore(Guid userId, Guid fileItemId, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("update-recognition-state")]
        public async Task<IActionResult> UpdateRecognitionState(object updateModel)
        {
            throw new NotImplementedException();
        }
    }
}
