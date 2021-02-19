using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads;
using Voicipher.Domain.Transcription;

namespace Voicipher.Business.Commands
{
    public class CreateUserSubscriptionCommand : Command<CreateUserSubscriptionPayload, CommandResult<TimeSpanWrapperOutputModel>>, ICreateUserSubscriptionCommand
    {
        private readonly IModifySubscriptionTimeCommand _modifySubscriptionTimeCommand;
        private readonly IBillingPurchaseRepository _billingPurchaseRepository;
        private readonly ICurrentUserSubscriptionRepository _currentUserSubscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CreateUserSubscriptionCommand(
            IModifySubscriptionTimeCommand modifySubscriptionTimeCommand,
            IBillingPurchaseRepository billingPurchaseRepository,
            ICurrentUserSubscriptionRepository currentUserSubscriptionRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger logger)
        {
            _modifySubscriptionTimeCommand = modifySubscriptionTimeCommand;
            _billingPurchaseRepository = billingPurchaseRepository;
            _currentUserSubscriptionRepository = currentUserSubscriptionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger.ForContext<CreateUserSubscriptionCommand>();
        }

        protected override async Task<CommandResult<TimeSpanWrapperOutputModel>> Execute(CreateUserSubscriptionPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();

            _logger.Information($"[{userId}] Start billing purchase registration. Product ID = {parameter.ProductId}");
            if (!parameter.Validate().IsValid)
            {
                _logger.Error($"[{userId}] Invalid input data");
                throw new OperationErrorException(ErrorCode.EC600);
            }

            var billingPurchase = _mapper.Map<BillingPurchase>(parameter);
            if (!billingPurchase.Validate().IsValid)
            {
                _logger.Error($"[{userId}] Invalid billing purchase input data");
                throw new OperationErrorException(ErrorCode.EC600);
            }

            if (userId != parameter.UserId)
                throw new OperationErrorException(ErrorCode.EC301);

            try
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
                {
                    var isSuccess = await RegisterPurchaseAsync(billingPurchase, parameter.ApplicationId, principal, cancellationToken);
                    if (!isSuccess)
                        throw new OperationErrorException(ErrorCode.EC302);

                    _logger.Information($"[{userId}] Billing purchase was successfully registered. Billing purchase: {billingPurchase.Id}");

                    await transaction.CommitAsync(cancellationToken);

                    var remainingTime = await _currentUserSubscriptionRepository.GetRemainingTimeAsync(userId, cancellationToken);
                    return new CommandResult<TimeSpanWrapperOutputModel>(new TimeSpanWrapperOutputModel(remainingTime.Ticks));
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.Error(ex, "An error occurred while updating the entries");

                throw new OperationErrorException(ErrorCode.EC400);
            }
        }

        private async Task<bool> RegisterPurchaseAsync(BillingPurchase billingPurchase, Guid applicationId, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            await _billingPurchaseRepository.AddAsync(billingPurchase);
            await _unitOfWork.SaveAsync(cancellationToken);

            var userId = principal.GetNameIdentifier();
            var subscriptionProduct = SubscriptionProducts.All.FirstOrDefault(x => x.Id == billingPurchase.ProductId);
            if (subscriptionProduct == null)
            {
                _logger.Error($"[{userId}] Product ID {billingPurchase.ProductId} not exists");
                return false;
            }

            var modifySubscriptionTimePayload = new ModifySubscriptionTimePayload
            {
                UserId = billingPurchase.UserId,
                ApplicationId = applicationId,
                Time = subscriptionProduct.Time,
                Operation = SubscriptionOperation.Add
            };

            var commandResult = await _modifySubscriptionTimeCommand.ExecuteAsync(modifySubscriptionTimePayload, principal, cancellationToken);
            if (!commandResult.IsSuccess)
            {
                _logger.Error($"[{userId}] User subscription was not modified. Error code = {commandResult.Error.ErrorCode}");

                return false;
            }

            _logger.Information($"[{userId}] Purchase was registered. Purchase ID = {billingPurchase.Id}, Subscription time = {subscriptionProduct.Time}");

            return true;
        }
    }
}
