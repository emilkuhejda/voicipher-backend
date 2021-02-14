using System;

namespace Voicipher.Domain.Models
{
    public record DeletedAudioFile
    {
        public Guid Id { get; set; }

        public DateTime DeletedDate { get; set; }
    }
}
