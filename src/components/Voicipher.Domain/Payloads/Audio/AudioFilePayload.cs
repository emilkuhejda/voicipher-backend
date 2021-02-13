using System;

namespace Voicipher.Domain.Payloads.Audio
{
    public record AudioFilesPayload
    {
        public AudioFilesPayload(DateTime updatedAfter, Guid applicationId)
        {
            UpdatedAfter = updatedAfter;
            ApplicationId = applicationId;
        }

        public DateTime UpdatedAfter { get; }

        public Guid ApplicationId { get; }
    }
}
