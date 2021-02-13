using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/mail")]
    [Authorize(Policy = nameof(VoicipherPolicy.User))]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class MailController : ControllerBase
    {
        [HttpPost]
        public IActionResult SendEmail([FromBody] object mailModel)
        {
            throw new NotImplementedException();
        }
    }
}
