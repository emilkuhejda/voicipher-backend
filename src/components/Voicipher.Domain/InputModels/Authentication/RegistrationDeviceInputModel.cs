using System;
using System.ComponentModel.DataAnnotations;
using Voicipher.Domain.Interfaces.Validation;
using ValidationResult = Voicipher.Domain.Validation.ValidationResult;

namespace Voicipher.Domain.InputModels.Authentication
{
    public class RegistrationDeviceInputModel : IValidatable
    {
        [Required]
        public Guid InstallationId { get; set; }

        [Required]
        public string RuntimePlatform { get; set; }

        [Required]
        public string InstalledVersionNumber { get; set; }

        [Required]
        public string Language { get; set; }

        public ValidationResult Validate()
        {
            return ValidationResult.Success;
        }
    }
}
