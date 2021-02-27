using System;
using Voicipher.Domain.Enums;

namespace Voicipher.Domain.Models
{
    public class BackgroundJob : EntityBase
    {
        public Guid UserId { get; set; }

        public Guid AudioFileId { get; set; }

        public JobState JobState { get; set; }

        public int Attempt { get; set; }

        public string Parameters { get; set; }

        public string Exception { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateCompletedUtc { get; set; }
    }
}
