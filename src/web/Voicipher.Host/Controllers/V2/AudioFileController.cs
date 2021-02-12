using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.InputModels.Audio;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/audio-files")]
    [Produces("application/json")]
    [Authorize(Policy = nameof(VoicipherPolicy.User))]
    [ApiController]
    public class AudioFileController : ControllerBase
    {
        private readonly Lazy<ICreateAudioFileCommand> _createAudioFileCommand;

        public AudioFileController(Lazy<ICreateAudioFileCommand> createAudioFileCommand)
        {
            _createAudioFileCommand = createAudioFileCommand;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetAudioFiles")]
        public IActionResult GetAudioFiles()
        {
            return Ok();
        }

        [HttpPost("create")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AudioFileOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "CreateAudioFile")]
        public async Task<IActionResult> CreateAudioFile([FromQuery] CreateAudioFileInputModel createAudioFileInputModel, CancellationToken cancellationToken)
        {
            var validationResult = createAudioFileInputModel.Validate();
            if (!validationResult.IsValid)
                throw new OperationErrorException(ErrorCode.EC600);

            var commandResult = await _createAudioFileCommand.Value.ExecuteAsync(createAudioFileInputModel, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }
    }
}
