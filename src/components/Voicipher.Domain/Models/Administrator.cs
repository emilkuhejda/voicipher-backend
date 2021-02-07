using System.Collections.Generic;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class Administrator : EntityBase, IValidatable
    {
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id));
            errors.ValidateRequired(Username, nameof(Username));
            errors.ValidateRequired(FirstName, nameof(FirstName));
            errors.ValidateRequired(LastName, nameof(LastName));
            errors.ValidateRequired(PasswordHash, nameof(PasswordHash));
            errors.ValidateRequired(PasswordSalt, nameof(PasswordSalt));

            return new ValidationResult(errors);
        }
    }
}
