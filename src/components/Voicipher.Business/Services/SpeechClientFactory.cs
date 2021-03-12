using Google.Apis.Auth.OAuth2;
using Google.Cloud.Speech.V1;
using Grpc.Auth;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Services
{
    public class SpeechClientFactory : ISpeechClientFactory
    {
        private readonly AppSettings _appSettings;

        public SpeechClientFactory(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
        }

        public SpeechClient CreateClient()
        {
            var serializedCredentials = JsonConvert.SerializeObject(_appSettings.SpeechCredentials);
            var credentials = GoogleCredential
                .FromJson(serializedCredentials)
                .CreateScoped(_appSettings.GoogleApiAuthUri);

            var builder = new SpeechClientBuilder
            {
                ChannelCredentials = credentials.ToChannelCredentials()
            };

            return builder.Build();
        }
    }
}
