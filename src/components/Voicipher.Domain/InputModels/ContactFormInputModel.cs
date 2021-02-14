using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;
using ValidationResult = Voicipher.Domain.Validation.ValidationResult;

namespace Voicipher.Domain.InputModels
{
    public record ContactFormInputModel : IValidatable
    {
        [Required]
        public string Name { get; init; }

        [Required]
        public string Email { get; init; }

        [Required]
        public string Message { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateRequired(Name, nameof(Name));
            errors.ValidateRequired(Email, nameof(Email));
            errors.ValidateEmail(Email, nameof(Email));
            errors.ValidateRequired(Message, nameof(Message));

            return new ValidationResult(errors);
        }
    }
}
