using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/meta-data")]
    [ApiController]
    public class MetaDataController : ControllerBase
    {
        [HttpGet(".well-known/openid-configuration")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = "GetOpenIdConfiguration")]
        public IActionResult GetOpenIdConfiguration()
        {
            return new OkObjectResult(new
            {
                issuer = "https://appleid.apple.com",
                authorization_endpoint = "https://appleid.apple.com/auth/authorize",
                token_endpoint = "https://appleid.apple.com/auth/token",
                jwks_uri = "https://appleid.apple.com/auth/keys",
                id_token_signing_alg_values_supported = new[] { "RS256" }
            });
        }
    }
}
