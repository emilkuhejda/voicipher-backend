﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Google.Api.Gax;
using Google.Api.Gax.Grpc;
using Google.Cloud.Speech.V1;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Transcription;
using Voicipher.Domain.Utils;

namespace Voicipher.Business.Services
{
    public class SpeechRecognitionService : SpeechRecognitionServiceBase
    {
        public SpeechRecognitionService(
            ISpeechClientFactory speechClientFactory,
            IAudioFileProcessingChannel audioFileProcessingChannel,
            IMessageCenterService messageCenterService,
            IFileAccessService fileAccessService,
            IIndex<StorageLocation, IDiskStorage> index,
            ILogger logger)
            : base(speechClientFactory, audioFileProcessingChannel, messageCenterService, fileAccessService, index, logger)
        {
        }

        protected override async Task<RecognizedResult> GetRecognizedResultAsync(SpeechClient speechClient, TranscribedAudioFile transcribedAudioFile, SpeechRecognizeConfig speechRecognizeConfig)
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

                var longOperation = await speechClient.LongRunningRecognizeAsync(recognitionConfig, recognitionAudio, CallSettings.FromExpiration(Expiration.None));
                longOperation = await longOperation.PollUntilCompletedAsync();
                var longRunningRecognizeResponse = longOperation.Result;

                var alternatives = longRunningRecognizeResponse.Results
                    .Select((result, index) => new
                    {
                        Alternatives = result.Alternatives.Select(x => new RecognitionAlternative(index, x.Transcript, x.Confidence, x.Words.ToRecognitionWords()))
                    })
                    .SelectMany(x => x.Alternatives);

                return new RecognizedResult(false, alternatives);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, $"[{speechRecognizeConfig.UserId}] Recognition failed");

                return new RecognizedResult(true, Enumerable.Empty<RecognitionAlternative>());
            }
        }
    }
}
