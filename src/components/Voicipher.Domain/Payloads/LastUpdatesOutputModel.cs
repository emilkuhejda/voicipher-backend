using System;
using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.Payloads
{
    public record LastUpdatesOutputModel
    {
        [Required]
        public DateTime FileItemUtc { get; init; }

        [Required]
        public DateTime DeletedFileItemUtc { get; init; }

        [Required]
        public DateTime TranscribeItemUtc { get; init; }

        [Required]
        public DateTime UserSubscriptionUtc { get; init; }

        [Required]
        public DateTime InformationMessageUtc { get; init; }

        [Required]
        public string ApiUrl { get; set; }

        [Required]
        public Version ApiVersion { get; set; }
    }
}
