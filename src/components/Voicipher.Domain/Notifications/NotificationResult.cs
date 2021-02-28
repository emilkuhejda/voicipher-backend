using Newtonsoft.Json;

namespace Voicipher.Domain.Notifications
{
    [JsonObject]
    public record NotificationResult
    {
        [JsonProperty("notification_id")]
        public string Id { get; set; }
    }
}
