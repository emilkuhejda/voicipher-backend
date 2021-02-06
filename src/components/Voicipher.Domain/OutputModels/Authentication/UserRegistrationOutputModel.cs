using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.OutputModels.Authentication
{
    public record UserRegistrationOutputModel
    {
        [Required]
        public string Token { get; init; }

        [Required]
        public string RefreshToken { get; init; }

        public IdentityOutputModel Identity { get; init; }

        public TimeSpanWrapperOutputModel RemainingTime { get; init; }
    }
}
