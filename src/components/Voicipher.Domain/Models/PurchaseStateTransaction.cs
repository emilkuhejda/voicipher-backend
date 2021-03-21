using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Voicipher.Common.Utils;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class PurchaseStateTransaction : EntityBase, IValidatable
    {
        public Guid BillingPurchaseId { get; set; }

        public string PreviousPurchaseState { get; set; }

        public string PurchaseState { get; set; }

        [NotMapped]
        public PurchaseState State => EnumHelper.Parse(PurchaseState, Enums.PurchaseState.Unknown);

        public DateTime TransactionDateUtc { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id), nameof(PurchaseStateTransaction));
            errors.ValidateGuid(BillingPurchaseId, nameof(BillingPurchaseId), nameof(PurchaseStateTransaction));
            errors.ValidateRequired(PurchaseState, nameof(PurchaseState), nameof(PurchaseStateTransaction));
            errors.ValidateMaxLength(PurchaseState, nameof(PurchaseState), 250, nameof(PurchaseStateTransaction));
            errors.ValidateDateTime(TransactionDateUtc, nameof(TransactionDateUtc), nameof(PurchaseStateTransaction));

            if (!string.IsNullOrWhiteSpace(PreviousPurchaseState))
            {
                errors.ValidateMaxLength(PreviousPurchaseState, nameof(PreviousPurchaseState), 250, nameof(PurchaseStateTransaction));
            }

            return new ValidationResult(errors);
        }
    }
}
