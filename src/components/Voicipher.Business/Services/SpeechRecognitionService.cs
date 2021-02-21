using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Speech.V1;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Settings;
using Voicipher.Domain.Transcription;
using Voicipher.Domain.Utils;

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

        protected override async Task<RecognizedResult> GetRecognizedResultAsync(SpeechClient speech, TranscribedAudioFile transcribedAudioFile, SpeechRecognizeConfig speechRecognizeConfig)
        {
            try
            {
                var recognitionConfig = new RecognitionConfig
                {
                    LanguageCode = speechRecognizeConfig.Language,
                    EnableAutomaticPunctuation = true,
                    EnableWordTimeOffsets = true,
                    AudioChannelCount = transcribedAudioFile.AudioChannels,
                    EnableSeparateRecognitionPerChannel = true
                };

                if (speechRecognizeConfig.IsPhoneCall)
                {
                    recognitionConfig.UseEnhanced = true;
                    recognitionConfig.Model = RecognitionModel.PhoneCall;
                }

                var recognitionAudio = await RecognitionAudio.FromFileAsync(transcribedAudioFile.Path);

                var longOperation = await speech.LongRunningRecognizeAsync(recognitionConfig, recognitionAudio);
                longOperation = await longOperation.PollUntilCompletedAsync();
                var longRunningRecognizeResponse = longOperation.Result;

                var alternatives = longRunningRecognizeResponse.Results
                    .SelectMany(x => x.Alternatives)
                    .Select(x => new RecognitionAlternative(x.Transcript, x.Confidence, x.Words.ToRecognitionWords()));

                return new RecognizedResult(true, alternatives);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, $"[{speechRecognizeConfig.UserId}] Recognition failed");

                return new RecognizedResult(false, Enumerable.Empty<RecognitionAlternative>());
            }
        }
    }
}
