using System;
using System.Collections.Generic;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class User : EntityBase, IValidatable
    {
        public string Email { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public DateTime DateRegisteredUtc { get; set; }

        public CurrentUserSubscription CurrentUserSubscription { get; set; }

        public IList<UserSubscription> UserSubscriptions { get; set; }

        //public IList<object> FileItems { get; set; }

        //public IList<object> RecognizedAudioSamples { get; set; }

        //public IList<object> BillingPurchases { get; set; }

        //public IList<object> InformationMessages { get; set; }

        //public IList<object> UserDevices { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id), nameof(User));

            errors.ValidateRequired(Email, nameof(Email), nameof(User));
            errors.ValidateMaxLength(Email, nameof(Email), 100, nameof(User));

            errors.ValidateRequired(GivenName, nameof(GivenName), nameof(User));
            errors.ValidateMaxLength(GivenName, nameof(GivenName), 100, nameof(User));

            errors.ValidateRequired(FamilyName, nameof(FamilyName), nameof(User));
            errors.ValidateMaxLength(FamilyName, nameof(FamilyName), 100, nameof(User));

            errors.ValidateDateTime(DateRegisteredUtc, nameof(DateRegisteredUtc), nameof(User));

            return new ValidationResult(errors);
        }
    }
}
