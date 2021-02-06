using System;

namespace Voicipher.Domain.Models
{
    public class User : EntityBase
    {
        public string Email { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public DateTime DateRegisteredUtc { get; set; }
    }
}
