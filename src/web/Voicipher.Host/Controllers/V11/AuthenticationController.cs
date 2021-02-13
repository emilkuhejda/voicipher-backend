﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Domain.InputModels.Authentication;
using Voicipher.Domain.Interfaces.Commands.Authentication;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.OutputModels.Authentication;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V11
{
    [ApiVersion("1.1")]
    [ApiExplorerSettings(GroupName = "v1.1")]
    [Route("api/v{version:apiVersion}/auth")]
    [Produces("application/json")]
    [Authorize(Policy = nameof(VoicipherPolicy.AzureAd))]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly Lazy<IUserRegistrationCommand> _userRegistrationCommand;
        private readonly Lazy<IMapper> _mapper;

        public AuthenticationController(
            Lazy<IUserRegistrationCommand> userRegistrationCommand,
            Lazy<IMapper> mapper)
        {
            _userRegistrationCommand = userRegistrationCommand;
            _mapper = mapper;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(UserRegistrationOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationInputModel registrationUserRegistrationModel, CancellationToken cancellationToken)
        {
            var commandResult = await _userRegistrationCommand.Value.ExecuteAsync(registrationUserRegistrationModel, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                return BadRequest(_mapper.Value.Map<ErrorResultOutputModel>(commandResult));

            return Ok(commandResult.Value);
        }
    }
}