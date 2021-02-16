using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.Business.Infrastructure;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Payloads;
using Voicipher.Domain.Validation;

namespace Voicipher.Business.Commands.Transcription
{
    public class CanRunRecognitionCommand : Command<CanRunRecognitionPayload, CommandResult>, ICanRunRecognitionCommand
    {
        private readonly ICurrentUserSubscriptionRepository _currentUserSubscriptionRepository;

        public CanRunRecognitionCommand(ICurrentUserSubscriptionRepository currentUserSubscriptionRepository)
        {
            _currentUserSubscriptionRepository = currentUserSubscriptionRepository;
        }

        protected override async Task<CommandResult> Execute(CanRunRecognitionPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var subscriptionRemainingTime = await _currentUserSubscriptionRepository.GetRemainingTimeAsync(parameter.UserId, cancellationToken);
            if (subscriptionRemainingTime.TotalSeconds < 1)
                return new CommandResult(new OperationError(ValidationErrorCodes.NotEnoughSubscriptionTime));

            return new CommandResult();
        }
    }
}
