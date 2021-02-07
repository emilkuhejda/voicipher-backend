using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Business.Extensions;
using Voicipher.Domain.InputModels.Authentication;
using Voicipher.Domain.Interfaces.Commands.Authentication;
using Voicipher.Domain.OutputModels.Authentication;

namespace Voicipher.Host.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/b2c/v{version:apiVersion}/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly Lazy<IUserRegistrationCommand> _userRegistrationCommand;
        private readonly ILogger _logger;

        public AuthenticationController(
            Lazy<IUserRegistrationCommand> userRegistrationCommand,
            ILogger logger)
        {
            _userRegistrationCommand = userRegistrationCommand;
            _logger = logger.ForContext<AuthenticationController>();
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(UserRegistrationOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationInputModel registrationUserRegistrationModel, CancellationToken cancellationToken)
        {
            var errors = registrationUserRegistrationModel.Validate();
            if (!errors.IsValid)
            {
                _logger.Error($"User input is invalid. {errors.ToJson()}");

                return BadRequest(errors);
            }

            var commandResult = await _userRegistrationCommand.Value.ExecuteAsync(registrationUserRegistrationModel, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
            {
                _logger.Error($"Validation errors during processing occurs. {commandResult.ValidationErrors.ToJson()}");

                return BadRequest(commandResult.ValidationErrors);
            }

            _logger.Information($"User '{registrationUserRegistrationModel.Email}' was successfully registered and token was created.");

            return Ok(commandResult.Value);
        }
    }
}
