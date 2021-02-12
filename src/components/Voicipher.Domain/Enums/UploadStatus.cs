using System.Text.Json.Serialization;

namespace Voicipher.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UploadStatus
    {
        None = 0,
        InProgress = 1,
        Completed = 2,
        Error = 3
    }
}
