using System;
using System.Collections.Generic;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.InputModels
{
    public class CreateUserSubscriptionInputModel : IValidatable
    {
        public Guid Id { get; set; }

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

            return new ValidationResult(errors);
        }
    }
}
