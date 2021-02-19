using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/contact-forms")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class ContactFormController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{contactFormId}")]
        public async Task<IActionResult> Get(Guid contactFormId)
        {
            throw new NotImplementedException();
        }
    }
}
