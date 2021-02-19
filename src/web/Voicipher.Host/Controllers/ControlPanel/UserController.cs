using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/users")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            throw new NotImplementedException();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser(Guid userId, string email)
        {
            throw new NotImplementedException();
        }
    }
}
