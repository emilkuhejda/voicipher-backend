using System;
using System.Collections.Generic;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class DeletedAccount : EntityBase, IValidatable
    {
        public Guid UserId { get; set; }

        public DateTime DateDeletedUtc { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id));
            errors.ValidateGuid(UserId, nameof(UserId));
            errors.ValidateDateTime(DateDeletedUtc, nameof(DateDeletedUtc));

            return new ValidationResult(errors);
        }
    }
}
