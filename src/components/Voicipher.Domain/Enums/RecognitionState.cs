using System.Text.Json.Serialization;

namespace Voicipher.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RecognitionState
    {
        None = 0,
        Converting = 1,
        Prepared = 2,
        InProgress = 3,
        Completed = 4
    }
}
