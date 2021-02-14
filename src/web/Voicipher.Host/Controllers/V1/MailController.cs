using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/mail")]
    [Authorize(Policy = nameof(VoicipherPolicy.User))]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly Lazy<ISendMailCommand> _sendMailCommand;

        public MailController(Lazy<ISendMailCommand> sendMailCommand)
        {
            _sendMailCommand = sendMailCommand;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] SendMailInputModel sendMailInputModel, CancellationToken cancellationToken)
        {
            var commandResult = await _sendMailCommand.Value.ExecuteAsync(sendMailInputModel, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }
    }
}
