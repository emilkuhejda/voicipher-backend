using System;
using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.InputModels.Audio
{
    public record DeletedAudioFileInputModel
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        public DateTime DeletedDate { get; init; }
    }
}
