using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.OutputModels.Audio
{
    public record RecognitionAlternativeOutputModel
    {
        public RecognitionAlternativeOutputModel()
        {
            Words = new List<RecognitionWordInfoOutputModel>();
        }

        [Required]
        public string Transcript { get; init; }

        [Required]
        public float Confidence { get; init; }

        public IList<RecognitionWordInfoOutputModel> Words { get; }
    }
}
