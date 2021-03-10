using Google.Cloud.Speech.V1;

namespace Voicipher.Business.Services
{
    public interface ISpeechClientFactory
    {
        SpeechClient CreateClient();
    }
}
