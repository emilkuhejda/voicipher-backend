using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Payloads;

namespace Voicipher.Business.Commands
{
    public class CreateUserSubscriptionCommand : Command<CreateUserSubscriptionPayload, CommandResult<TimeSpanWrapperOutputModel>>, ICreateUserSubscriptionCommand
    {
        private readonly IBillingPurchaseRepository _billingPurchaseRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CreateUserSubscriptionCommand(ILogger logger)
        {
            _logger = logger.ForContext<CreateUserSubscriptionCommand>();
        }

        protected override async Task<CommandResult<TimeSpanWrapperOutputModel>> Execute(CreateUserSubscriptionPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetNameIdentifier();
            if (!parameter.Validate().IsValid)
            {
                _logger.Error($"Invalid input data. User ID = {userId}.");
                throw new OperationErrorException(ErrorCode.EC600);
            }

            var billingPurchase = _mapper.Map<BillingPurchase>(parameter);
            if (!billingPurchase.Validate().IsValid)
            {
                _logger.Error($"Invalid billing purchase input data. User ID = {userId}.");
                throw new OperationErrorException(ErrorCode.EC600);
            }

            if (userId != parameter.UserId)
                throw new OperationErrorException(ErrorCode.EC301);

            await Task.CompletedTask;
            return new CommandResult<TimeSpanWrapperOutputModel>(new TimeSpanWrapperOutputModel(0));
        }
    }
}
