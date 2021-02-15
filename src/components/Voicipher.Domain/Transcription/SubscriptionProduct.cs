using System;

namespace Voicipher.Domain.Transcription
{
    public record SubscriptionProduct
    {
        public SubscriptionProduct(string id, TimeSpan time)
        {
            Id = id;
            Time = time;
        }

        public string Id { get; }

        public TimeSpan Time { get; }
    }
}
