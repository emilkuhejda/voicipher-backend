using System;
using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.OutputModels
{
    public record IdentityOutputModel
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        public string Email { get; init; }

        [Required]
        public string GivenName { get; init; }

        [Required]
        public string FamilyName { get; init; }
    }
}
