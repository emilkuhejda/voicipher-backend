﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.OutputModels;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/speech-results")]
    [Authorize(Policy = nameof(VoicipherPolicy.User))]
    [ApiController]
    public class SpeechResultsController : ControllerBase
    {
        private readonly Lazy<ICreateSpeechResultCommand> _createSpeechResultCommand;
        private readonly Lazy<IUpdateSpeechResultsCommand> _updateSpeechResultsCommand;

        public SpeechResultsController(
            Lazy<ICreateSpeechResultCommand> createSpeechResultCommand,
            Lazy<IUpdateSpeechResultsCommand> updateSpeechResultsCommand)
        {
            _createSpeechResultCommand = createSpeechResultCommand;
            _updateSpeechResultsCommand = updateSpeechResultsCommand;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(OkOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "CreateSpeechResult")]
        public async Task<IActionResult> Create(CreateSpeechResultInputModel createSpeechResultInputModel, CancellationToken cancellationToken)
        {
            var commandResult = await _createSpeechResultCommand.Value.ExecuteAsync(createSpeechResultInputModel, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpPut("update")]
        [ProducesResponseType(typeof(TimeSpanWrapperOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "UpdateSpeechResults")]
        public async Task<IActionResult> Update(IEnumerable<SpeechResultInputModel> speechResultInputModels, CancellationToken cancellationToken)
        {
            var commandResult = await _updateSpeechResultsCommand.Value.ExecuteAsync(speechResultInputModels.ToArray(), HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }
    }
}
