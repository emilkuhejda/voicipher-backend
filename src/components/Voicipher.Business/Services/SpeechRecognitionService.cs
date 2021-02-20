﻿using System.Threading.Tasks;
using Google.Cloud.Speech.V1;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Settings;
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

        protected override async Task<LongRunningRecognizeResponse> GetRecognizedResponseAsync(SpeechClient speech, TranscribedAudioFile transcribedAudioFile, SpeechRecognizeConfig speechRecognizeConfig)
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
            return longOperation.Result;
        }
    }
}
