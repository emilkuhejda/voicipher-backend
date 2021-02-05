using System;
using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.OutputModels
{
    public class IdentityOutputModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string GivenName { get; set; }

        [Required]
        public string FamilyName { get; set; }
    }
}
