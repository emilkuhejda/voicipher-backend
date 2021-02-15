using System;

namespace Voicipher.Domain.Transcription
{
    public static class SubscriptionProducts
    {
        public static SubscriptionProduct ProductBasic { get; } = new SubscriptionProduct("product.subscription.v1.basic", TimeSpan.FromHours(1));

        public static SubscriptionProduct ProductStandard { get; } = new SubscriptionProduct("product.subscription.v1.standard", TimeSpan.FromHours(5));

        public static SubscriptionProduct ProductAdvanced { get; } = new SubscriptionProduct("product.subscription.v1.advanced", TimeSpan.FromHours(10));

        public static SubscriptionProduct[] All { get; } = { ProductBasic, ProductStandard, ProductAdvanced };
    }
}
