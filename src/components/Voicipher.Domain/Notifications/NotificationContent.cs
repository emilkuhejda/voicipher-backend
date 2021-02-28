using System.Collections.Generic;
using Newtonsoft.Json;

namespace Voicipher.Domain.Notifications
{
    [JsonObject]
    public record NotificationContent
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("custom_data")]
        public IDictionary<string, string> CustomData { get; set; }
    }
}
