using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/last-updates")]
    [Authorize(Policy = nameof(VoicipherPolicy.User))]
    [ApiController]
    public class LastUpdatesController : ControllerBase
    {
        [HttpGet]
        // [ProducesResponseType(typeof(LastUpdatesDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetLastUpdates")]
        public ActionResult Get()
        {
            return Ok(new
            {
                FileItemUtc = new DateTime(2020, 1, 1, 0, 0, 0),
                DeletedFileItemUtc = new DateTime(2020, 1, 1, 0, 0, 0),
                TranscribeItemUtc = new DateTime(2020, 1, 1, 0, 0, 0),
                UserSubscriptionUtc = new DateTime(2020, 1, 1, 0, 0, 0),
                InformationMessageUtc = new DateTime(2020, 1, 1, 0, 0, 0)
            });
        }
    }
}
