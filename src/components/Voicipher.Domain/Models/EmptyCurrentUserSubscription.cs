using System;

namespace Voicipher.Domain.Models
{
    public class EmptyCurrentUserSubscription : CurrentUserSubscription
    {
        public EmptyCurrentUserSubscription()
        {
            Id = Guid.NewGuid();
            UserId = Guid.Empty;
            Ticks = TimeSpan.Zero.Ticks;
            DateUpdatedUtc = DateTime.Now;
        }
    }
}
