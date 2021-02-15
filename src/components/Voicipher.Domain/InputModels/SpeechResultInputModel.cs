using System;
using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.InputModels
{
    public record SpeechResultInputModel
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        public long Ticks { get; init; }
    }
}
