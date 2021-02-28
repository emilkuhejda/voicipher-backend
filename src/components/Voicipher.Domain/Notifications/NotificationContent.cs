using System.Collections.Generic;
using Newtonsoft.Json;

namespace Voicipher.Domain.Notifications
{
    [JsonObject]
    public record NotificationContent
    {
        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("title")]
        public string Title { get; init; }

        [JsonProperty("body")]
        public string Body { get; init; }

        [JsonProperty("custom_data")]
        public IDictionary<string, string> CustomData { get; init; }
    }
}
