using System;
using System.Collections.Generic;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Payloads
{
    public record CreateUserSubscriptionPayload : IValidatable
    {
        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public Guid ApplicationId { get; init; }

        public string PurchaseId { get; init; }

        public string ProductId { get; init; }

        public bool AutoRenewing { get; init; }

        public string PurchaseToken { get; init; }

        public string PurchaseState { get; init; }

        public string ConsumptionState { get; init; }

        public string Platform { get; init; }

        public DateTime TransactionDateUtc { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id));
            errors.ValidateGuid(UserId, nameof(UserId));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));

            return new ValidationResult(errors);
        }
    }
}
