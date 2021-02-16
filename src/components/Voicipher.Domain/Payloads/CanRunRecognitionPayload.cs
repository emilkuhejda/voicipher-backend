using System;

namespace Voicipher.Domain.Payloads
{
    public record CanRunRecognitionPayload
    {
        public CanRunRecognitionPayload(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }
    }
}
