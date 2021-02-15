using System;
using System.ComponentModel.DataAnnotations;

namespace Voicipher.Domain.InputModels
{
    public record SpeechResultInputModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public long Ticks { get; set; }
    }
}
