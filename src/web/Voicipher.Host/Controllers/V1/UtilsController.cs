using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Domain.InputModels.MetaData;
using Voicipher.Domain.Interfaces.Commands.ControlPanel;
using Voicipher.Domain.Interfaces.Queries.ControlPanel;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/utils")]
    [ApiController]
    public class UtilsController : ControllerBase
    {
        private readonly Lazy<IGetAdministratorQuery> _getAdministratorQuery;
        private readonly Lazy<IGenerateTokenCommand> _generateTokenCommand;
        private readonly Lazy<IMapper> _mapper;

        public UtilsController(
            Lazy<IGetAdministratorQuery> getAdministratorQuery,
            Lazy<IGenerateTokenCommand> generateTokenCommand,
            Lazy<IMapper> mapper)
        {
            _getAdministratorQuery = getAdministratorQuery;
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "RefreshToken")]
        public IActionResult RefreshToken()
        {
            throw new NotImplementedException();
        }

        [HttpPost("generate-token")]
        public async Task<IActionResult> CreateToken([FromForm] CreateTokenInputModel createTokenInputModel, CancellationToken cancellationToken)
        {
            var queryResult = await _getAdministratorQuery.Value.ExecuteAsync(createTokenInputModel, null, cancellationToken);
            if (!queryResult.IsSuccess)
                return BadRequest(_mapper.Value.Map<ErrorResultOutputModel>(queryResult));

            var payload = _mapper.Value.Map(createTokenInputModel, _mapper.Value.Map<GenerateTokenPayload>(queryResult.Value));
            var commandResult = await _generateTokenCommand.Value.ExecuteAsync(payload, null, cancellationToken);
            if (!commandResult.IsSuccess)
                return BadRequest(_mapper.Value.Map<ErrorResultOutputModel>(commandResult));

            return Ok(commandResult.Value);
        }
    }
}
