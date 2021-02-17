using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands
{
    public class ModifySubscriptionTimeCommand : Command<ModifySubscriptionTimePayload, CommandResult>, IModifySubscriptionTimeCommand
    {
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly ICurrentUserSubscriptionRepository _currentUserSubscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ModifySubscriptionTimeCommand(
            IUserSubscriptionRepository userSubscriptionRepository,
            ICurrentUserSubscriptionRepository currentUserSubscriptionRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger logger)
        {
            _userSubscriptionRepository = userSubscriptionRepository;
            _currentUserSubscriptionRepository = currentUserSubscriptionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger.ForContext<ModifySubscriptionTimeCommand>();
        }

        protected override async Task<CommandResult> Execute(ModifySubscriptionTimePayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = parameter.UserId;
            var userSubscription = _mapper.Map<UserSubscription>(parameter);

            if (!userSubscription.Validate().IsValid)
            {
                _logger.Error("Invalid subscription input data.");

                return new CommandResult(new OperationError(ValidationErrorCodes.InvalidDateTime));
            }

            var userSubscriptions = (await _userSubscriptionRepository.GetAllAsync(userId, cancellationToken)).ToList();
            var remainingTicks = userSubscriptions.CalculateRemainingTicks();
            if (parameter.Operation == SubscriptionOperation.Remove && remainingTicks < userSubscription.Time.Ticks)
            {
                _logger.Error($"Not enough subscription time for user ID = {userId}. Required time: {parameter.Time}, Remaining time: {TimeSpan.FromTicks(remainingTicks)}.");

                return new CommandResult(new OperationError(ValidationErrorCodes.NotEnoughSubscriptionTime));
            }

            await _userSubscriptionRepository.AddAsync(userSubscription);

            userSubscriptions.Add(userSubscription);
            var newRemainingTicks = userSubscriptions.CalculateRemainingTicks();

            var currentUserSubscriptions = await _currentUserSubscriptionRepository.GetByUserIdAsync(userId, cancellationToken);
            _currentUserSubscriptionRepository.RemoveRange(currentUserSubscriptions);

            var currentUserSubscription = new CurrentUserSubscription(userId, newRemainingTicks);
            await _currentUserSubscriptionRepository.AddAsync(currentUserSubscription);

            await _unitOfWork.SaveAsync(cancellationToken);

            _logger.Information($"Current user subscription was updated to time: {currentUserSubscription.Time}. User ID = {userId}");

            return new CommandResult();
        }
    }
}
