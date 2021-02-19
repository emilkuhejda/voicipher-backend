using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/utils")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class UtilsController : ControllerBase
    {
        private readonly Lazy<ISpeechRecognitionService> _speechRecognitionService;

        public UtilsController(Lazy<ISpeechRecognitionService> speechRecognitionService)
        {
            _speechRecognitionService = speechRecognitionService;
        }

        [HttpPut("send-message")]
        [ApiExplorerSettings(IgnoreApi = false)]
        public async Task<IActionResult> SendMessage(Guid userId, Guid fileItemId, double percentage)
        {
            throw new NotImplementedException();
        }

        [HttpGet("has-access")]
        public IActionResult HasAccess()
        {
            return Ok(true);
        }

        [HttpGet("is-deployment-successful")]
        public IActionResult IsDeploymentSuccessful()
        {
            var canCreateSpeechClient = _speechRecognitionService.Value.CanCreateSpeechClientAsync();
            if (!canCreateSpeechClient)
                return BadRequest();

            return Ok();
        }

        [HttpGet("generate-hangfire-access")]
        public IActionResult GenerateHangfireAccess()
        {
            throw new NotImplementedException();
        }

        [HttpPut("reset-database")]
        public async Task<IActionResult> ResetDatabase([FromBody] object resetDatabaseModel)
        {
            throw new NotImplementedException();
        }

        [HttpPut("delete-database")]
        public async Task<IActionResult> DeleteDatabase([FromBody] object resetDatabaseModel)
        {
            throw new NotImplementedException();
        }
    }
}
