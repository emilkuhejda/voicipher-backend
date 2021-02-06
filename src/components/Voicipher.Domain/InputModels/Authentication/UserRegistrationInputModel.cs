using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;
using ValidationResult = Voicipher.Domain.Validation.ValidationResult;

namespace Voicipher.Domain.InputModels.Authentication
{
    public record UserRegistrationInputModel : IValidatable
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        public Guid ApplicationId { get; init; }

        [Required]
        public string Email { get; init; }

        public string GivenName { get; init; }

        public string FamilyName { get; init; }

        public RegistrationDeviceInputModel Device { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(Id, nameof(Id), nameof(UserRegistrationInputModel));
            errors.ValidateGuid(ApplicationId, nameof(ApplicationId));

            errors.ValidateRequired(Email, nameof(Email));

            errors.Merge(Device.Validate());

            return new ValidationResult(errors);
        }
    }
}
