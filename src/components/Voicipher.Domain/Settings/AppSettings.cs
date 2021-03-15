using System;

namespace Voicipher.Domain.Settings
{
    public class AppSettings
    {
        public string ApiUrl { get; set; }

        public Version ApiVersion { get; set; }

        public Guid ApplicationId { get; set; }

        public string SecretKey { get; set; }

        public string ConnectionString { get; set; }

        public Authentication Authentication { get; set; }

        public AzureStorageAccountSettings AzureStorageAccount { get; set; }

        public MailConfiguration MailConfiguration { get; set; }

        public AzureRecognitionSpeechConfiguration AzureSpeechConfiguration { get; set; }

        public string GoogleApiAuthUri { get; set; }

        public SpeechCredentials SpeechCredentials { get; set; }

        public string[] AllowedHosts { get; set; }
    }
}
