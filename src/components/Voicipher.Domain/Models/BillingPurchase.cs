using System;
using System.Collections.Generic;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class BillingPurchase : EntityBase, IValidatable
    {
        public Guid UserId { get; set; }

        public string PurchaseId { get; set; }

        public string ProductId { get; set; }

        public bool AutoRenewing { get; set; }

        public string PurchaseToken { get; set; }

        public string PurchaseState { get; set; }

        public string ConsumptionState { get; set; }

        public string Platform { get; set; }

        public DateTime TransactionDateUtc { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id));
            errors.ValidateGuid(UserId, nameof(UserId));
            errors.ValidateRequired(PurchaseId, nameof(PurchaseId));
            errors.ValidateRequired(ProductId, nameof(ProductId));
            errors.ValidateMaxLength(ProductId, nameof(ProductId), 250);
            errors.ValidateRequired(PurchaseToken, nameof(PurchaseToken));
            errors.ValidateRequired(PurchaseState, nameof(PurchaseState));
            errors.ValidateMaxLength(PurchaseState, nameof(PurchaseState), 250);
            errors.ValidateMaxLength(ConsumptionState, nameof(ConsumptionState), 250);
            errors.ValidateRequired(Platform, nameof(Platform));
            errors.ValidateMaxLength(Platform, nameof(Platform), 250);
            errors.ValidateDateTime(TransactionDateUtc, nameof(TransactionDateUtc));

            return new ValidationResult(errors);
        }
    }
}
