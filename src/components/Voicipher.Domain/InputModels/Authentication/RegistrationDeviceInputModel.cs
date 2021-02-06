using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;
using ValidationResult = Voicipher.Domain.Validation.ValidationResult;

namespace Voicipher.Domain.InputModels.Authentication
{
    public record RegistrationDeviceInputModel : IValidatable
    {
        [Required]
        public Guid InstallationId { get; init; }

        [Required]
        public string RuntimePlatform { get; init; }

        [Required]
        public string InstalledVersionNumber { get; init; }

        [Required]
        public string Language { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateGuid(InstallationId, nameof(InstallationId));
            errors.ValidateRequired(RuntimePlatform, nameof(RuntimePlatform));
            errors.ValidateRequired(InstalledVersionNumber, nameof(InstalledVersionNumber));
            errors.ValidateRequired(Language, nameof(Language));

            return new ValidationResult(errors);
        }
    }
}
