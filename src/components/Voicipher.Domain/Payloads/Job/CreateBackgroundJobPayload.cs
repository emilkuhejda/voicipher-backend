using System;
using System.Collections.Generic;
using Voicipher.Domain.Enums;

namespace Voicipher.Domain.Payloads.Job
{
    public record CreateBackgroundJobPayload
    {
        public CreateBackgroundJobPayload(Guid userId, Guid audioFileId, Guid jobId, Dictionary<BackgroundJobParameter, object> parameters)
        {
            UserId = userId;
            AudioFileId = audioFileId;
            JobId = jobId;
            Parameters = parameters;
        }

        public Guid UserId { get; }

        public Guid AudioFileId { get; }

        public Guid JobId { get; }

        public Dictionary<BackgroundJobParameter, object> Parameters { get; }
    }
}
