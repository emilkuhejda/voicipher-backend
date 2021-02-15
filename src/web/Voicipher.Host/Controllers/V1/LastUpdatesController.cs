using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Interfaces.Queries;
using Voicipher.Domain.Payloads;
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
        private readonly Lazy<IGetLastUpdatesQuery> _getLastUpdatesQuery;

        public LastUpdatesController(Lazy<IGetLastUpdatesQuery> getLastUpdatesQuery)
        {
            _getLastUpdatesQuery = getLastUpdatesQuery;
        }

        [HttpGet]
        [ProducesResponseType(typeof(LastUpdatesOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetLastUpdates")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var queryResult = await _getLastUpdatesQuery.Value.ExecuteAsync(HttpContext.User, cancellationToken);
            if (!queryResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(queryResult.Value);
        }
    }
}
