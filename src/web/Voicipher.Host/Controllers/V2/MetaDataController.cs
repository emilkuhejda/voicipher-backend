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
using Voicipher.Domain.OutputModels;

namespace Voicipher.Host.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/meta-data")]
    [ApiController]
    public class MetaDataController : ControllerBase
    {
        private readonly Lazy<IGenerateTokenCommand> _getAdministratorQuery;
        private readonly Lazy<IMapper> _mapper;

        public MetaDataController(
            Lazy<IGenerateTokenCommand> getAdministratorQuery,
            Lazy<IMapper> mapper)
        {
            _getAdministratorQuery = getAdministratorQuery;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("is-alive")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = "IsAlive")]
        public IActionResult IsAlive()
        {
            return Ok(true);
        }

        [AllowAnonymous]
        [HttpPost("generate-token")]
        public async Task<IActionResult> CreateToken([FromForm] CreateTokenInputModel createTokenInputModel, CancellationToken cancellationToken)
        {
            var queryResult = await _getAdministratorQuery.Value.ExecuteAsync(createTokenInputModel, null, cancellationToken);
            if (!queryResult.IsSuccess)
                return BadRequest(_mapper.Value.Map<ErrorResultOutputModel>(queryResult));

            return Ok(queryResult.Value);
        }
    }
}
