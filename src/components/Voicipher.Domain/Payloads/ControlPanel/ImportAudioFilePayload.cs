namespace Voicipher.Domain.Payloads.ControlPanel
{
    public record ImportAudioFilePayload
    {
        public string UsersJsonPath { get; init; }

        public string SubscriptionsJsonPath { get; init; }

        public string AlternativesJsonPath { get; init; }

        public string UploadsDirectoryPath { get; init; }
    }
}
