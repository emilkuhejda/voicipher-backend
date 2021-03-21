using System;
using System.Collections.Generic;
using System.Linq;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class BillingPurchase : EntityBase, IValidatable
    {
        public BillingPurchase()
        {
            PurchaseStateTransactions = new List<PurchaseStateTransaction>();
        }

        public Guid UserId { get; set; }

        public string PurchaseId { get; set; }

        public string ProductId { get; set; }

        public bool AutoRenewing { get; set; }

        public string PurchaseToken { get; set; }

        public string ConsumptionState { get; set; }

        public string Platform { get; set; }

        public DateTime TransactionDateUtc { get; set; }

        public PurchaseState PurchaseState => PurchaseStateTransactions.Any()
            ? PurchaseStateTransactions.OrderByDescending(x => x.TransactionDateUtc).FirstOrDefault()?.State ?? PurchaseState.Unknown
            : PurchaseState.Unknown;

        public IList<PurchaseStateTransaction> PurchaseStateTransactions { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id), nameof(BillingPurchase));
            errors.ValidateGuid(UserId, nameof(UserId), nameof(BillingPurchase));
            errors.ValidateRequired(PurchaseId, nameof(PurchaseId), nameof(BillingPurchase));
            errors.ValidateRequired(ProductId, nameof(ProductId), nameof(BillingPurchase));
            errors.ValidateMaxLength(ProductId, nameof(ProductId), 250, nameof(BillingPurchase));
            errors.ValidateRequired(PurchaseToken, nameof(PurchaseToken), nameof(BillingPurchase));
            errors.ValidateMaxLength(ConsumptionState, nameof(ConsumptionState), 250, nameof(BillingPurchase));
            errors.ValidateRequired(Platform, nameof(Platform), nameof(BillingPurchase));
            errors.ValidateMaxLength(Platform, nameof(Platform), 250, nameof(BillingPurchase));
            errors.ValidateDateTime(TransactionDateUtc, nameof(TransactionDateUtc), nameof(BillingPurchase));

            errors.Merge(PurchaseStateTransactions.Select(x => x.Validate()).ToList());

            return new ValidationResult(errors);
        }
    }
}
