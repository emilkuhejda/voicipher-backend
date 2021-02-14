using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands
{
    public class SendMailCommand : Command<SendMailInputModel, CommandResult<OkOutputModel>>, ISendMailCommand
    {
        private readonly ILogger _logger;

        public SendMailCommand(ILogger logger)
        {
            _logger = logger.ForContext<SendMailCommand>();
        }

        protected override async Task<CommandResult<OkOutputModel>> Execute(SendMailInputModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var validationResult = parameter.Validate();
            if (!validationResult.IsValid)
            {
                if (validationResult.Errors.ContainsError(nameof(SendMailInputModel.Recipient), ValidationErrorCodes.InvalidEmail))
                {
                    _logger.Error($"Recipient email address format '{parameter.Recipient}' is incorrect.");

                    throw new OperationErrorException(ErrorCode.EC202);
                }

                _logger.Error("Invalid input data.");

                throw new OperationErrorException(ErrorCode.EC600);
            }


            await Task.CompletedTask;
            return new CommandResult<OkOutputModel>(new OkOutputModel());
        }
    }
}
