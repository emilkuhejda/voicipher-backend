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
using Voicipher.Domain.InputModels.MetaData;
using Voicipher.Domain.Interfaces.Commands.Authentication;
using Voicipher.Domain.Interfaces.Commands.ControlPanel;
using Voicipher.Domain.OutputModels;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/utils")]
    [ApiController]
    public class UtilsController : ControllerBase
    {
        private readonly Lazy<IRefreshTokenCommand> _refreshTokenCommand;
        private readonly Lazy<IGenerateTokenCommand> _generateTokenCommand;
        private readonly Lazy<IMapper> _mapper;

        public UtilsController(
            Lazy<IRefreshTokenCommand> refreshTokenCommand,
            Lazy<IGenerateTokenCommand> generateTokenCommand,
            Lazy<IMapper> mapper)
        {
            _refreshTokenCommand = refreshTokenCommand;
            _generateTokenCommand = generateTokenCommand;
            _mapper = mapper;
        }

        [HttpGet("is-alive")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = "IsAlive")]
        public IActionResult IsAlive()
        {
            return Ok(true);
        }

        [HttpGet("refresh-token")]
        [Authorize(Policy = nameof(VoicipherPolicy.Security))]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "RefreshToken")]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            var commandResult = await _refreshTokenCommand.Value.ExecuteAsync(TimeSpan.FromDays(180), HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpPost("generate-token")]
        public async Task<IActionResult> CreateToken([FromForm] CreateTokenInputModel createTokenInputModel, CancellationToken cancellationToken)
        {
            var commandResult = await _generateTokenCommand.Value.ExecuteAsync(createTokenInputModel, null, cancellationToken);
            if (!commandResult.IsSuccess)
                return BadRequest(_mapper.Value.Map<ErrorResultOutputModel>(commandResult));

            return Ok(commandResult.Value);
        }
    }
}
