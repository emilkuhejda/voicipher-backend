using System;

namespace Voicipher.Domain.InputModels
{
    public record CreateUserSubscriptionInputModel
    {
        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public string PurchaseId { get; init; }

        public string ProductId { get; init; }

        public bool AutoRenewing { get; init; }

        public string PurchaseToken { get; init; }

        public string PurchaseState { get; init; }

        public string ConsumptionState { get; init; }

        public string Platform { get; init; }

        public DateTime TransactionDateUtc { get; init; }
    }
}
