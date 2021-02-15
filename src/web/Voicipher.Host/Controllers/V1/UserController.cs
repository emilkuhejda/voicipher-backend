using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.InputModels.Authentication;
using Voicipher.Domain.InputModels.EndUser;
using Voicipher.Domain.Interfaces.Commands.Authentication;
using Voicipher.Domain.Interfaces.Commands.EndUser;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.OutputModels.Authentication;
using Voicipher.Domain.Payloads.EndUser;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/users")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Lazy<IUpdateUserCommand> _updateUserCommand;
        private readonly Lazy<IUserRegistrationCommand> _userRegistrationCommand;
        private readonly Lazy<IUpdateLanguageCommand> _updateLanguageCommand;
        private readonly Lazy<IDeleteUserCommand> _deleteUserCommand;
        private readonly Lazy<IMapper> _mapper;

        public UserController(
            Lazy<IUpdateUserCommand> updateUserCommand,
            Lazy<IUserRegistrationCommand> userRegistrationCommand,
            Lazy<IUpdateLanguageCommand> updateLanguageCommand,
            Lazy<IDeleteUserCommand> deleteUserCommand, Lazy<IMapper> mapper)
        {
            _updateUserCommand = updateUserCommand;
            _userRegistrationCommand = userRegistrationCommand;
            _updateLanguageCommand = updateLanguageCommand;
            _deleteUserCommand = deleteUserCommand;
            _mapper = mapper;
        }

        [HttpPut("update")]
        [Authorize(Policy = nameof(VoicipherPolicy.User))]
        [ProducesResponseType(typeof(IdentityOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserInputModel updateUserInputModel, CancellationToken cancellationToken)
        {
            var commandResult = await _updateUserCommand.Value.ExecuteAsync(updateUserInputModel, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpPost("/api/b2c/v{version:apiVersion}/users/register")]
        [Authorize(Policy = nameof(VoicipherPolicy.AzureAd))]
        [ProducesResponseType(typeof(UserRegistrationOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "RegisterUser")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationInputModel registrationUserRegistrationModel, CancellationToken cancellationToken)
        {
            var commandResult = await _userRegistrationCommand.Value.ExecuteAsync(registrationUserRegistrationModel, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                return BadRequest(_mapper.Value.Map<ErrorResultOutputModel>(commandResult));

            return Ok(commandResult.Value);
        }

        [HttpPut("update-language")]
        [Authorize(Policy = nameof(VoicipherPolicy.User))]
        [ProducesResponseType(typeof(OkOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "UpdateLanguage")]
        public async Task<IActionResult> UpdateLanguage(Guid installationId, int language, CancellationToken cancellationToken)
        {
            var updateLanguagePayload = new UpdateLanguagePayload(installationId, language);
            var commandResult = await _updateLanguageCommand.Value.ExecuteAsync(updateLanguagePayload, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpDelete]
        [Authorize(Policy = nameof(VoicipherPolicy.User))]
        [ProducesResponseType(typeof(OkOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "DeleteUser")]
        public async Task<IActionResult> DeleteUser(string email, CancellationToken cancellationToken)
        {
            var commandResult = await _deleteUserCommand.Value.ExecuteAsync(email, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }
    }
}
