using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/administrators")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        [HttpGet("{administratorId}")]
        public async Task<IActionResult> Get(Guid administratorId)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            throw new NotImplementedException();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(object createAdministratorModel)
        {
            throw new NotImplementedException();
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(object updateAdministrator)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(Guid administratorId)
        {
            throw new NotImplementedException();
        }
    }
}
