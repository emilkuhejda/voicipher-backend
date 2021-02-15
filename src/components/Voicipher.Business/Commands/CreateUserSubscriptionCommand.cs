using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Commands
{
    public class CreateUserSubscriptionCommand : Command<CreateUserSubscriptionInputModel, CommandResult<TimeSpanWrapperOutputModel>>, ICreateUserSubscriptionCommand
    {
        private readonly ILogger _logger;

        public CreateUserSubscriptionCommand(ILogger logger)
        {
            _logger = logger.ForContext<CreateUserSubscriptionCommand>();
        }

        protected override async Task<CommandResult<TimeSpanWrapperOutputModel>> Execute(CreateUserSubscriptionInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if (!parameter.Validate().IsValid)
            {
                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }

            await Task.CompletedTask;
            return new CommandResult<TimeSpanWrapperOutputModel>(new TimeSpanWrapperOutputModel(0));
        }
    }
}
