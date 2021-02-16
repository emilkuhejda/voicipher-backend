using System;

namespace Voicipher.Domain.Models
{
    public class BackgroundJob : EntityBase
    {
        public Guid UserId { get; set; }

        public Guid AudioFileId { get; set; }

        public string Parameters { get; set; }

        public DateTime DateCreatedUtc { get; set; }
    }
}
