using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;
using ValidationResult = Voicipher.Domain.Validation.ValidationResult;

namespace Voicipher.Domain.InputModels.Authentication
{
    public class UserRegistrationInputModel : IValidatable
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ApplicationId { get; set; }

        [Required]
        public string Email { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public RegistrationDeviceInputModel Device { get; set; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();
            errors.Merge(Device.Validate());

            return new ValidationResult(errors);
        }
    }
}
