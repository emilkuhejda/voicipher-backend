using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voicipher.Domain.InputModels.ControlPanel;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Controllers.ControlPanel
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/control-panel/subscriptions")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = nameof(VoicipherPolicy.Admin))]
    [ApiController]
    public class UserSubscriptionController : ControllerBase
    {
        private readonly Lazy<IModifySubscriptionTimeCommand> _modifySubscriptionTimeCommand;
        private readonly Lazy<ICurrentUserSubscriptionRepository> _currentUserSubscriptionRepository;
        private readonly Lazy<IUserSubscriptionRepository> _userSubscriptionRepository;
        private readonly Lazy<IMapper> _mapper;

        public UserSubscriptionController(
            Lazy<IModifySubscriptionTimeCommand> modifySubscriptionTimeCommand,
            Lazy<ICurrentUserSubscriptionRepository> currentUserSubscriptionRepository,
            Lazy<IUserSubscriptionRepository> userSubscriptionRepository,
            Lazy<IMapper> mapper)
        {
            _modifySubscriptionTimeCommand = modifySubscriptionTimeCommand;
            _currentUserSubscriptionRepository = currentUserSubscriptionRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateSubscriptionInputModel createSubscriptionInputModel, CancellationToken cancellationToken)
        {
            var modifySubscriptionTimePayload = _mapper.Value.Map<ModifySubscriptionTimePayload>(createSubscriptionInputModel);
            var commandResult = await _modifySubscriptionTimeCommand.Value.ExecuteAsync(modifySubscriptionTimePayload, HttpContext.User, cancellationToken);
            if (!commandResult.IsSuccess)
                return BadRequest(_mapper.Value.Map<ErrorResultOutputModel>(commandResult));

            return Ok(commandResult.Value);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetSubscriptions(Guid userId, CancellationToken cancellationToken)
        {
            var userSubscriptions = await _userSubscriptionRepository.Value.GetAllAsync(userId, cancellationToken);
            var outputModels = userSubscriptions.Select(x => new
            {
                Id = x.Id,
                UserId = x.UserId,
                ApplicationId = x.ApplicationId,
                Time = x.Time,
                Operation = x.Operation,
                DateCreatedUtc = x.DateCreatedUtc
            }).ToArray();

            return Ok(outputModels);
        }

        [HttpGet("remaining-time/{userId}")]
        public async Task<IActionResult> GetSubscriptionRemainingTime(Guid userId, CancellationToken cancellationToken)
        {
            var remainingTime = await _currentUserSubscriptionRepository.Value.GetRemainingTimeAsync(userId, cancellationToken);

            return Ok(new TimeSpanWrapperOutputModel(remainingTime.Ticks));
        }
    }
}
