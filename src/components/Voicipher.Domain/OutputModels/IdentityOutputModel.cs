using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;
using ValidationResult = Voicipher.Domain.Validation.ValidationResult;

namespace Voicipher.Domain.OutputModels
{
    public record IdentityOutputModel : IValidatable
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        public string Email { get; init; }

        [Required]
        public string GivenName { get; init; }

        [Required]
        public string FamilyName { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id), nameof(IdentityOutputModel));
            errors.ValidateRequired(Email, nameof(Email), nameof(IdentityOutputModel));
            errors.ValidateRequired(GivenName, nameof(GivenName), nameof(IdentityOutputModel));
            errors.ValidateRequired(FamilyName, nameof(FamilyName), nameof(IdentityOutputModel));

            return new ValidationResult(errors);
        }
    }
}
