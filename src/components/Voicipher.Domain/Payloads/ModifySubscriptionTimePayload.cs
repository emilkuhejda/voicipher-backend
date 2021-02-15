using System;
using Voicipher.Domain.Enums;

namespace Voicipher.Domain.Payloads
{
    public record ModifySubscriptionTimePayload
    {
        public Guid ApplicationId { get; init; }

        public TimeSpan Time { get; init; }

        public SubscriptionOperation Operation { get; init; }
    }
}
