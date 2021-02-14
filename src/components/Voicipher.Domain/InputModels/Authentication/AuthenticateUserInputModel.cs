using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;
using ValidationResult = Voicipher.Domain.Validation.ValidationResult;

namespace Voicipher.Domain.InputModels.Authentication
{
    public record AuthenticateUserInputModel : IValidatable
    {
        [Required]
        public string Username { get; init; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateRequired(Username, nameof(Username));
            errors.ValidateRequired(Password, nameof(Password));

            return new ValidationResult(errors);
        }
    }
}
