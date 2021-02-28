using System.Collections;
using Newtonsoft.Json;

namespace Voicipher.Domain.Notifications
{
    [JsonObject]
    public record NotificationTarget
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("devices")]
        public IEnumerable Devices { get; set; }
    }
}
