using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.OutputModels.Authentication
{
    public class UserRegistrationOutputModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }

        public IdentityOutputModel Identity { get; set; }

        public TimeSpanWrapperOutputModel RemainingTime { get; set; }
    }
}
