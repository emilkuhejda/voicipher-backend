using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.OutputModels;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/cache")]
    [Authorize(Policy = nameof(VoicipherPolicy.User))]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly Lazy<IAudioFileProcessingChannel> _audioFileProcessingChannel;

        public CacheController(Lazy<IAudioFileProcessingChannel> audioFileProcessingChannel)
        {
            _audioFileProcessingChannel = audioFileProcessingChannel;
        }

        [HttpGet("{fileItemId}")]
        [ProducesResponseType(typeof(CacheItemOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetPercentage")]
        public IActionResult GetCacheItem(Guid fileItemId)
        {
            var progress = _audioFileProcessingChannel.Value.GetProgress(fileItemId);
            var recognitionState = progress.HasValue ? RecognitionState.InProgress : RecognitionState.None;
            var cacheItemOutputModel = new CacheItemOutputModel(fileItemId, recognitionState, progress ?? 0);
            return Ok(cacheItemOutputModel);
        }
    }
}
