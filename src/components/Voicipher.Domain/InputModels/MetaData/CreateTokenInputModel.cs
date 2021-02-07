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
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Role Role { get; set; }

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
