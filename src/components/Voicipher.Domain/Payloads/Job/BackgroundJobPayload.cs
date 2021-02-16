using System;
using System.Collections.Generic;
using Voicipher.Domain.Enums;

namespace Voicipher.Domain.Payloads.Job
{
    public record BackgroundJobPayload
    {
        public BackgroundJobPayload()
        {
            Parameters = new Dictionary<BackgroundJobParameter, object>();
        }

        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public Guid AudioFileId { get; init; }

        public Dictionary<BackgroundJobParameter, object> Parameters { get; }

        public DateTime DateCreatedUtc { get; init; }
    }
}
