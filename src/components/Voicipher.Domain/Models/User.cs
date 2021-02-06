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

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id));

            errors.ValidateRequired(Email, nameof(Email));
            errors.ValidateMaxLength(Email, nameof(Email), 100);

            errors.ValidateRequired(GivenName, nameof(GivenName));
            errors.ValidateMaxLength(GivenName, nameof(GivenName), 100);

            errors.ValidateRequired(FamilyName, nameof(FamilyName));
            errors.ValidateMaxLength(FamilyName, nameof(FamilyName), 100);

            errors.ValidateDateTime(DateRegisteredUtc, nameof(DateRegisteredUtc));

            return new ValidationResult(errors);
        }
    }
}
