using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Voicipher.Domain.OutputModels.Audio
{
    public record RecognitionAlternativeOutputModel
    {
        public RecognitionAlternativeOutputModel()
        {
            Words = new List<RecognitionWordInfoOutputModel>();
        }

        [Required]
        public int ResultNumber { get; init; }

        [Required]
        public string Transcript { get; init; }

        [Required]
        public float Confidence { get; init; }

        [Required]
        public TimeSpan StatTime => TimeSpan.FromTicks(Words.FirstOrDefault()?.StartTimeTicks ?? 0);

        public IList<RecognitionWordInfoOutputModel> Words { get; }
    }
}
