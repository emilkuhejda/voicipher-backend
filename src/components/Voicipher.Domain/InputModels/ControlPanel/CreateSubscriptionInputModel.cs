using System;
using System.ComponentModel.DataAnnotations;
using Voicipher.Domain.Enums;

namespace Voicipher.Domain.InputModels.ControlPanel
{
    public record CreateSubscriptionInputModel
    {
        [Required]
        public Guid UserId { get; init; }

        [Required]
        public Guid ApplicationId { get; init; }

        [Required]
        public SubscriptionOperation Operation { get; init; }

        [Required]
        public int Seconds { get; init; }
    }
}
