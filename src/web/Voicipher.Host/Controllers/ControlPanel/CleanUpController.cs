using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Interfaces.Commands.ControlPanel;
using Voicipher.Domain.Payloads.ControlPanel;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/clean-up")]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    [AllowAnonymous]
    public class CleanUpController : ControllerBase
    {
        private readonly Lazy<ICleanUpAudioFilesCommand> _cleanUpAudioFilesCommand;
        private readonly Lazy<ICleanUpTranscribeItemsCommand> _cleanUpTranscribeItemsCommand;

        public CleanUpController(
            Lazy<ICleanUpAudioFilesCommand> cleanUpAudioFilesCommand,
            Lazy<ICleanUpTranscribeItemsCommand> cleanUpTranscribeItemsCommand)
        {
            _cleanUpAudioFilesCommand = cleanUpAudioFilesCommand;
            _cleanUpTranscribeItemsCommand = cleanUpTranscribeItemsCommand;
        }

        [HttpPost("audio-files")]
        public async Task<IActionResult> CleanUpAudioFiles(CancellationToken cancellationToken)
        {
            var commandResult = await _cleanUpAudioFilesCommand.Value.ExecuteAsync(new CleanUpAudioFilesPayload(), HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpPost("transcribe-item")]
        public async Task<IActionResult> CleanUpTranscribeItems(CancellationToken cancellationToken)
        {
            var commandResult = await _cleanUpTranscribeItemsCommand.Value.ExecuteAsync(new CleanUpTranscribeItemsPayload(), HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }
    }
}
