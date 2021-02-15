using System;
using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.OutputModels
{
    public class SpeechConfigurationOutputModel
    {
        [Required]
        public string SubscriptionKey { get; init; }

        [Required]
        public string SpeechRegion { get; init; }

        [Required]
        public Guid AudioSampleId { get; init; }

        [Required]
        public long SubscriptionRemainingTimeTicks { get; init; }
    }
}
