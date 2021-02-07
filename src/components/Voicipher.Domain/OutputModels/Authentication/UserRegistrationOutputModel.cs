using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Voicipher.Domain.Interfaces.Validation;
using Voicipher.Domain.Validation;
using ValidationResult = Voicipher.Domain.Validation.ValidationResult;

namespace Voicipher.Domain.OutputModels.Authentication
{
    public record UserRegistrationOutputModel : IValidatable
    {
        [Required]
        public string Token { get; init; }

        [Required]
        public string RefreshToken { get; init; }

        public IdentityOutputModel Identity { get; init; }

        public TimeSpanWrapperOutputModel RemainingTime { get; init; }

        public ValidationResult Validate()
        {
            IList<ValidationError> errors = new List<ValidationError>();

            errors.ValidateRequired(Token, nameof(Token), nameof(UserRegistrationOutputModel));
            errors.ValidateRequired(RefreshToken, nameof(RefreshToken), nameof(UserRegistrationOutputModel));
            errors.ValidateRequired(Identity, nameof(Identity), nameof(UserRegistrationOutputModel));

            if (Identity != null)
            {
                errors.Merge(Identity.Validate());
            }

            return new ValidationResult(errors);
        }
    }
}
