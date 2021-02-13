﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.OutputModels.Audio;
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V11
{
    [ApiVersion("1.1")]
    [ApiExplorerSettings(GroupName = "v1.1")]
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
        public async Task<IActionResult> CreateAudioFile(string name, string language, string fileName, DateTime dateCreated, Guid applicationId, CancellationToken cancellationToken)
        {
            var createAudioFilePayload = new CreateAudioFilePayload
            {
                Name = name,
                Language = language,
                FileName = fileName,
                DateCreated = dateCreated,
                ApplicationId = applicationId
            };

            var commandResult = await _createAudioFileCommand.Value.ExecuteAsync(createAudioFilePayload, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }
    }
}