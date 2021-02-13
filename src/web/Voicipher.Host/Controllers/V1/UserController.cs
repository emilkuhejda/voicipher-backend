using System;
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

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/users")]
    [Produces("application/json")]
    [Authorize(Policy = nameof(VoicipherPolicy.AzureAd))]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Lazy<IUserRegistrationCommand> _userRegistrationCommand;
        private readonly Lazy<IMapper> _mapper;

        public UserController(
            Lazy<IUserRegistrationCommand> userRegistrationCommand,
            Lazy<IMapper> mapper)
        {
            _userRegistrationCommand = userRegistrationCommand;
            _mapper = mapper;
        }

        [HttpPost("/api/b2c/v{version:apiVersion}/users/register")]
        [ProducesResponseType(typeof(UserRegistrationOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "RegisterUser")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationInputModel registrationUserRegistrationModel, CancellationToken cancellationToken)
        {
            var commandResult = await _userRegistrationCommand.Value.ExecuteAsync(registrationUserRegistrationModel, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                return BadRequest(_mapper.Value.Map<ErrorResultOutputModel>(commandResult));

            return Ok(commandResult.Value);
        }
    }
}
