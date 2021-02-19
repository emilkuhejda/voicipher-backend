using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/subscriptions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class UserSubscriptionController : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] object createSubscriptionModel)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetSubscriptions(Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("remaining-time/{userId}")]
        public async Task<IActionResult> GetSubscriptionRemainingTime(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
