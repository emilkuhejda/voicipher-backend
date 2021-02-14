using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.InputModels.Authentication;
using Voicipher.Domain.Interfaces.Commands.Authentication;
using Voicipher.Domain.OutputModels.MetaData;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/authenticate")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly Lazy<IAuthenticateUserCommand> _authenticateUserCommand;

        public AuthenticationController(Lazy<IAuthenticateUserCommand> authenticateUserCommand)
        {
            _authenticateUserCommand = authenticateUserCommand;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AdministratorOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateUserInputModel authenticateUserInputModel, CancellationToken cancellationToken)
        {
            var commandResult = await _authenticateUserCommand.Value.ExecuteAsync(authenticateUserInputModel, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }
    }
}
