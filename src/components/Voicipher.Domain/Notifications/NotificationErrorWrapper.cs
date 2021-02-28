using Newtonsoft.Json;

namespace Voicipher.Domain.Notifications
{
    [JsonObject]
    public record NotificationErrorWrapper
    {
        [JsonProperty("error")]
        public NotificationError Error { get; set; }
    }
}
