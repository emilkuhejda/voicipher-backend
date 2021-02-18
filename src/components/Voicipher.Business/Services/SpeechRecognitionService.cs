using System.Threading.Tasks;
using Google.Cloud.Speech.V1;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Services
{
    public class SpeechRecognitionService : SpeechRecognitionServiceBase
    {
        public SpeechRecognitionService(
            IAudioFileProcessingChannel audioFileProcessingChannel,
            IMessageCenterService messageCenterService,
            IOptions<AppSettings> options,
            ILogger logger)
            : base(audioFileProcessingChannel, messageCenterService, options, logger)
        {
        }

        protected override async Task<LongRunningRecognizeResponse> GetRecognizedResponseAsync(SpeechClient speech, TranscribedAudioFile transcribedAudioFile, string language)
        {
            var recognitionConfig = new RecognitionConfig
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                LanguageCode = language,
                EnableAutomaticPunctuation = true,
                UseEnhanced = true,
                EnableWordTimeOffsets = true,
                AudioChannelCount = transcribedAudioFile.AudioChannels,
                EnableSeparateRecognitionPerChannel = true
            };
            var recognitionAudio = await RecognitionAudio.FromFileAsync(transcribedAudioFile.Path);

            var longOperation = await speech.LongRunningRecognizeAsync(recognitionConfig, recognitionAudio);
            longOperation = await longOperation.PollUntilCompletedAsync();
            return longOperation.Result;
        }
    }
}
