using System;
using System.ComponentModel.DataAnnotations;
using Voicipher.Domain.Enums;

namespace Voicipher.Domain.OutputModels.Audio
{
    public record FileItemOutputModel
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        public string Name { get; init; }

        [Required]
        public string FileName { get; init; }

        [Required]
        public string Language { get; init; }

        [Required]
        public bool IsPhoneCall { get; set; }

        [Required]
        public string RecognitionStateString { get; init; }

        [Required]
        public UploadStatus UploadStatus { get; init; }

        [Required]
        public long TotalTimeTicks { get; init; }

        [Required]
        public long TranscribedTimeTicks { get; init; }

        [Required]
        public DateTime DateCreated { get; init; }

        public DateTime? DateProcessedUtc { get; init; }

        [Required]
        public DateTime DateUpdatedUtc { get; init; }

        [Required]
        public bool IsDeleted { get; init; }
    }
}
