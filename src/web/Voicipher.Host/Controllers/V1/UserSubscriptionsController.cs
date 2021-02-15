using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Business.Extensions;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.V1
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/subscriptions")]
    [Authorize(Policy = nameof(VoicipherPolicy.User))]
    [ApiController]
    public class UserSubscriptionsController : ControllerBase
    {
        private readonly Lazy<ICreateUserSubscriptionCommand> _createUserSubscriptionCommand;
        private readonly Lazy<ICreateSpeechConfigurationCommand> _createSpeechConfigurationCommand;
        private readonly Lazy<ICurrentUserSubscriptionRepository> _currentUserSubscriptionRepository;

        public UserSubscriptionsController(
            Lazy<ICreateUserSubscriptionCommand> createUserSubscriptionCommand,
            Lazy<ICreateSpeechConfigurationCommand> createSpeechConfigurationCommand,
            Lazy<ICurrentUserSubscriptionRepository> currentUserSubscriptionRepository)
        {
            _createUserSubscriptionCommand = createUserSubscriptionCommand;
            _createSpeechConfigurationCommand = createSpeechConfigurationCommand;
            _currentUserSubscriptionRepository = currentUserSubscriptionRepository;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(TimeSpanWrapperOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "CreateUserSubscription")]
        public async Task<IActionResult> Create([FromBody] CreateUserSubscriptionInputModel createUserSubscriptionInputModel, Guid applicationId, CancellationToken cancellationToken)
        {
            var appId = applicationId;
            var commandResult = await _createUserSubscriptionCommand.Value.ExecuteAsync(createUserSubscriptionInputModel, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpGet("speech-configuration")]
        [ProducesResponseType(typeof(SpeechConfigurationOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetSpeechConfiguration")]
        public async Task<IActionResult> GetSpeechConfiguration(CancellationToken cancellationToken)
        {
            var commandResult = await _createSpeechConfigurationCommand.Value.ExecuteAsync(Guid.NewGuid(), HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                throw new OperationErrorException(ErrorCode.EC601);

            return Ok(commandResult.Value);
        }

        [HttpGet("remaining-time")]
        [ProducesResponseType(typeof(TimeSpanWrapperOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetSubscriptionRemainingTime")]
        public async Task<IActionResult> GetSubscriptionRemainingTime(CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.GetNameIdentifier();
            var remainingTime = await _currentUserSubscriptionRepository.Value.GetRemainingTimeAsync(userId, cancellationToken);
            var outputModel = new TimeSpanWrapperOutputModel(remainingTime.Ticks);

            return Ok(outputModel);
        }
    }
}
