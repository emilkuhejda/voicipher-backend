using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.InputModels.ControlPanel;
using Voicipher.Domain.Interfaces.Commands.ControlPanel;
using Voicipher.Domain.Payloads.ControlPanel;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/audio-import")]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class AudioImportController : ControllerBase
    {
        private readonly Lazy<IImportAudioFileCommand> _importAudioFileCommand;
        private readonly Lazy<IMapper> _mapper;

        public AudioImportController(
            Lazy<IImportAudioFileCommand> importAudioFileCommand,
            Lazy<IMapper> mapper)
        {
            _importAudioFileCommand = importAudioFileCommand;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Import([FromBody] ImportAudioFileInputModel importAudioFileInputModel, CancellationToken cancellationToken)
        {
            var importAudioFilePayload = _mapper.Value.Map<ImportAudioFilePayload>(importAudioFileInputModel);
            var commandResult = await _importAudioFileCommand.Value.ExecuteAsync(importAudioFilePayload, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }
    }
}
