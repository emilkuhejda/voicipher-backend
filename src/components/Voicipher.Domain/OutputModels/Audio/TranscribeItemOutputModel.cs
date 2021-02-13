using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.OutputModels.Audio
{
    public record TranscribeItemOutputModel
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        public Guid FileItemId { get; init; }

        [Required]
        public IList<RecognitionAlternativeOutputModel> Alternatives { get; init; }

        public string UserTranscript { get; init; }

        [Required]
        public long StartTimeTicks { get; init; }

        [Required]
        public long EndTimeTicks { get; init; }

        [Required]
        public long TotalTimeTicks { get; init; }

        [Required]
        public DateTime DateCreatedUtc { get; init; }

        [Required]
        public DateTime DateUpdatedUtc { get; init; }
    }
}
