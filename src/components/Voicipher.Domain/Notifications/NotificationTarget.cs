using System.Collections;
using Newtonsoft.Json;

namespace Voicipher.Domain.Notifications
{
    [JsonObject]
    public record NotificationTarget
    {
        [JsonProperty("type")]
        public string Type { get; init; }

        [JsonProperty("devices")]
        public IEnumerable Devices { get; init; }
    }
}
