using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.OutputModels.Audio
{
    public record RecognitionWordInfoOutputModel
    {
        [Required]
        public string Word { get; init; }

        [Required]
        public long StartTimeTicks { get; init; }

        [Required]
        public long EndTimeTicks { get; init; }

        [Required]
        public int SpeakerTag { get; init; }
    }
}
