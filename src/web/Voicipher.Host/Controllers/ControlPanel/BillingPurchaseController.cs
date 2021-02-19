using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/purchases")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class BillingPurchaseController : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAll(Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("detail/{purchaseId}")]
        public async Task<IActionResult> Get(Guid purchaseId)
        {
            throw new NotImplementedException();
        }
    }
}
