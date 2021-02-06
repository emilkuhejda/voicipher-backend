using System;
using System.Collections.Generic;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class UserSubscription : EntityBase, IValidatable
    {
        public Guid UserId { get; set; }

        public Guid ApplicationId { get; set; }

        public TimeSpan Time { get; set; }

        public SubscriptionOperation Operation { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id));
            errors.ValidateGuid(UserId, nameof(UserId));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));
            errors.ValidateDateTime(DateCreatedUtc, nameof(DateCreatedUtc));

            return new ValidationResult(errors);
        }
    }
}
