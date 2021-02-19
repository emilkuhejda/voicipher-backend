namespace Voicipher.Domain.InputModels.ControlPanel
{
    public record ImportAudioFileInputModel
    {
        public string UsersJsonPath { get; init; }

        public string SubscriptionsJsonPath { get; init; }

        public string AlternativesJsonPath { get; init; }

        public string UploadsDirectoryPath { get; init; }
    }
}
