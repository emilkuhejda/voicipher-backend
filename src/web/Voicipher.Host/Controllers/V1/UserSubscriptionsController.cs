using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voicipher.Business.Extensions;
using Voicipher.Domain.Enums;
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
        private readonly Lazy<ICurrentUserSubscriptionRepository> _currentUserSubscriptionRepository;

        public UserSubscriptionsController(Lazy<ICurrentUserSubscriptionRepository> currentUserSubscriptionRepository)
        {
            _currentUserSubscriptionRepository = currentUserSubscriptionRepository;
        }

        [HttpPost("create")]
        // [ProducesResponseType(typeof(TimeSpanWrapperDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorCode), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "CreateUserSubscription")]
        public IActionResult Create([FromBody] object billingPurchase, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("speech-configuration")]
        // [ProducesResponseType(typeof(SpeechConfigurationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(OperationId = "GetSpeechConfiguration")]
        public IActionResult GetSpeechConfiguration()
        {
            throw new NotImplementedException();
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
