﻿using System;

namespace Voicipher.Domain.Models
{
    public record RecognitionFile
    {
        public RecognitionFile(Guid userId, Guid audioFileId)
        {
            UserId = userId;
            AudioFileId = audioFileId;
            DateProcessedUtc = DateTime.UtcNow;
        }

        public Guid UserId { get; }

        public Guid AudioFileId { get; }

        public DateTime DateProcessedUtc { get; }
    }
}
