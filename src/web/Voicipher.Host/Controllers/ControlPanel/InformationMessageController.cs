using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Domain.Enums;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/information-messages")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class InformationMessageController : ControllerBase
    {
        [HttpGet("{informationMessageId}")]
        public async Task<IActionResult> Get(Guid informationMessageId)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            throw new NotImplementedException();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] object informationMessageModel)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{informationMessageId}")]
        public async Task<IActionResult> Update(Guid informationMessageId, [FromForm] object informationMessageModel)
        {
            throw new NotImplementedException();
        }

        [HttpPut("send")]
        public async Task<IActionResult> SendNotification([FromForm] Guid informationMessageId, [FromForm] RuntimePlatform runtimePlatform, [FromForm] Language language)
        {
            throw new NotImplementedException();
        }
    }
}
