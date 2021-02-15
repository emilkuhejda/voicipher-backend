using System;
using System.Collections.Generic;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Extensions
{
    public static class UserSubscriptionExtensions
    {
        public static long CalculateRemainingTicks(this IEnumerable<UserSubscription> userSubscriptions)
        {
            var time = TimeSpan.Zero;

            foreach (var userSubscription in userSubscriptions)
            {
                if (userSubscription.Operation == SubscriptionOperation.Add)
                {
                    time = time.Add(userSubscription.Time);
                }
                else if (userSubscription.Operation == SubscriptionOperation.Remove)
                {
                    time = time.Subtract(userSubscription.Time);
                }
                else
                {
                    throw new NotSupportedException(nameof(userSubscription.Operation));
                }
            }

            return time.Ticks;
        }
    }
}
