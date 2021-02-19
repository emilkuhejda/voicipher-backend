using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/azure-storage")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class AzureStorageController : ControllerBase
    {
        [HttpPatch("migrate")]
        public IActionResult Migrate()
        {
            throw new NotImplementedException();
        }
    }
}
