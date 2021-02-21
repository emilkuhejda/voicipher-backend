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
    public class FakeSpeechRecognitionService : SpeechRecognitionServiceBase
    {
        public FakeSpeechRecognitionService(
            IAudioFileProcessingChannel audioFileProcessingChannel,
            IMessageCenterService messageCenterService,
            IOptions<AppSettings> options,
            ILogger logger)
            : base(audioFileProcessingChannel, messageCenterService, options, logger)
        {
        }

        protected override async Task<RecognizedResult> GetRecognizedResultAsync(SpeechClient speechClient, TranscribedAudioFile transcribedAudioFile, SpeechRecognizeConfig speechRecognizeConfig)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            try
            {
                var longRunningRecognizeResponse = new LongRunningRecognizeResponse
                {
                    Results =
                    {
                        new SpeechRecognitionResult
                        {
                            Alternatives =
                            {
                                new SpeechRecognitionAlternative
                                {
                                    Confidence = 0.99f,
                                    Transcript = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum."
                                }
                            }
                        }
                    }
                };

                var alternatives = longRunningRecognizeResponse.Results
                    .SelectMany(x => x.Alternatives)
                    .Select(x => new RecognitionAlternative(x.Transcript, x.Confidence, x.Words.ToRecognitionWords()));

                return new RecognizedResult(false, alternatives);
            }
            catch (Exception)
            {
                return new RecognizedResult(true, Enumerable.Empty<RecognitionAlternative>());
            }
        }
    }
}
