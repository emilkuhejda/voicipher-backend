using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.OutputModels.Audio
{
    public record RecognitionAlternativeOutputModel
    {
        [Required]
        public string Transcript { get; init; }

        [Required]
        public float Confidence { get; init; }

        public IList<RecognitionWordInfoOutputModel> Words { get; init; }
    }
}
