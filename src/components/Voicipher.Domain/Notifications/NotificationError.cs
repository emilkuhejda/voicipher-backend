using System.Net;
using Newtonsoft.Json;

namespace Voicipher.Domain.Notifications
{
    [JsonObject]
    public record NotificationError
    {
        [JsonProperty("code")]
        public string Code { get; init; }

        [JsonProperty("statusCode")]
        public HttpStatusCode StatusCode { get; init; }

        [JsonProperty("message")]
        public string Message { get; init; }
    }
}
