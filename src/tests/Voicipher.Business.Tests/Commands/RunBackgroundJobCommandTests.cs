using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Google.Cloud.Speech.V1;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using Xunit;

namespace Voicipher.Business.Tests.Commands
{
    public class RunBackgroundJobCommandTests
    {
        [Fact]
        public async Task ExecuteCommand()
        {
            var results = await GetSpeechRecognitionResults();
        }

        private async Task<RepeatedField<SpeechRecognitionResult>> GetSpeechRecognitionResults()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var pathToJson = Path.Combine(directory, "JsonData", "alternatives.json");
            var jsonText = await File.ReadAllTextAsync(pathToJson);

            return JsonConvert.DeserializeObject<RepeatedField<SpeechRecognitionResult>>(jsonText);
        }
    }
}
