using System;
using Voicipher.Domain.Enums;

namespace Voicipher.Domain.Payloads
{
    public record ModifySubscriptionTimePayload
    {
        public Guid UserId { get; init; }

        public Guid ApplicationId { get; init; }

        public TimeSpan Time { get; init; }

        public SubscriptionOperation Operation { get; init; }
    }
}
