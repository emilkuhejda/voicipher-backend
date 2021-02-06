using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class CurrentUserSubscription : EntityBase, IValidatable
    {
        public Guid UserId { get; set; }

        public long Ticks { get; set; }

        [NotMapped]
        public TimeSpan Time => TimeSpan.FromTicks(Ticks);

        public DateTime DateUpdatedUtc { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id), nameof(CurrentUserSubscription));
            errors.ValidateGuid(UserId, nameof(UserId), nameof(CurrentUserSubscription));
            errors.ValidateDateTime(DateUpdatedUtc, nameof(DateUpdatedUtc), nameof(CurrentUserSubscription));

            return new ValidationResult(errors);
        }
    }
}
