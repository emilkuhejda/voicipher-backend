using Newtonsoft.Json;

namespace Voicipher.Domain.Notifications
{
    [JsonObject]
    public record PushNotification
    {
        [JsonProperty("notification_target")]
        public NotificationTarget Target { get; init; }

        [JsonProperty("notification_content")]
        public NotificationContent Content { get; init; }
    }
}
