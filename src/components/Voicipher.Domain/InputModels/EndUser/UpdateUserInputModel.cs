using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.InputModels.EndUser
{
    public record UpdateUserInputModel
    {
        [Required]
        public string GivenName { get; init; }

        [Required]
        public string FamilyName { get; init; }
    }
}
