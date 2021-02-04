using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Business.Utils;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Settings;

namespace Voicipher.Host.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/meta-data")]
    [ApiController]
    public class MetaDataController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public MetaDataController(
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _appSettings = options.Value;
            _logger = logger;
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
        public IActionResult CreateToken()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, Role.User.ToString())
            };

            var token = TokenHelper.Generate(_appSettings.SecretKey, claims, TimeSpan.FromDays(180));

            _logger.Information($"Token was created.");

            return Ok(token);
        }
    }
}
