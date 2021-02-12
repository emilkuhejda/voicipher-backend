using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Domain.Enums;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/chunks")]
    [Produces("application/json")]
    [Authorize(Policy = nameof(VoicipherPolicy.User))]
    [ApiController]
    public class FileChunkController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(StorageConfigurationOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetChunksStorageConfiguration")]
        public IActionResult GetChunksStorageConfiguration()
        {
            return Ok(new StorageConfigurationOutputModel(StorageSetting.Azure));
        }
    }
}
