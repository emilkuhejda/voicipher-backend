using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;
using ValidationResult = Voicipher.Domain.Validation.ValidationResult;

namespace Voicipher.Domain.InputModels.MetaData
{
    public record CreateTokenInputModel : IValidatable
    {
        [Required]
        public string Username { get; init; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; init; }

        [Required]
        public Guid UserId { get; init; }

        [Required]
        public Role Role { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateRequired(Username, nameof(Username));
            errors.ValidateRequired(Password, nameof(Password));
            errors.ValidateGuid(UserId, nameof(UserId));

            return new ValidationResult(errors);
        }
    }
}
