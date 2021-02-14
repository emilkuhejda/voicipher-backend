using System;
using System.Collections.Generic;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;

namespace Voicipher.Domain.Models
{
    public class ContactForm : EntityBase, IValidatable
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id));
            errors.ValidateRequired(Name, nameof(Name));
            errors.ValidateMaxLength(Name, nameof(Name), 150);
            errors.ValidateRequired(Email, nameof(Email));
            errors.ValidateMaxLength(Email, nameof(Email), 150);
            errors.ValidateRequired(Message, nameof(Message));
            errors.ValidateDateTime(DateCreatedUtc, nameof(DateCreatedUtc));

            return new ValidationResult(errors);
        }
    }
}
