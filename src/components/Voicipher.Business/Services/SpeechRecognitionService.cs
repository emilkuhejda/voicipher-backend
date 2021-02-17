﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Speech.V1;
using Grpc.Auth;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Services
{
    public class SpeechRecognitionService : ISpeechRecognitionService
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public SpeechRecognitionService(
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _appSettings = options.Value;
            _logger = logger.ForContext<SpeechRecognitionService>();
        }

        public async Task<TranscribeItem[]> RecognizeAsync(TranscribeAudioFile[] transcribeAudioFiles, string language, CancellationToken cancellationToken)
        {
            var speechClient = CreateSpeechClient();
            var updateMethods = new List<Func<Task<TranscribeItem>>>();
            foreach (var audioFile in transcribeAudioFiles)
            {
                updateMethods.Add(() => RecognizeSpeech(speechClient, audioFile, language));
            }

            var transcribeItems = new List<TranscribeItem>();
            foreach (var enumerable in updateMethods.Split(10))
            {
                var tasks = enumerable.Select(x => x());
                var items = await Task.WhenAll(tasks);
                transcribeItems.AddRange(items);
            }

            return transcribeItems.ToArray();
        }

        private async Task<TranscribeItem> RecognizeSpeech(SpeechClient speech, TranscribeAudioFile transcribeAudioFile, string language)
        {
            _logger.Information($"Start speech recognition for file {transcribeAudioFile.Path}");

            var recognitionConfig = new RecognitionConfig
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                LanguageCode = language,
                EnableAutomaticPunctuation = true,
                UseEnhanced = true,
                EnableWordTimeOffsets = true,
                AudioChannelCount = transcribeAudioFile.AudioChannels,
                EnableSeparateRecognitionPerChannel = true
            };
            var recognitionAudio = await RecognitionAudio.FromFileAsync(transcribeAudioFile.Path);

            var longOperation = await speech.LongRunningRecognizeAsync(recognitionConfig, recognitionAudio);
            longOperation = await longOperation.PollUntilCompletedAsync().ConfigureAwait(false);
            var response = longOperation.Result;

            var alternatives = response.Results
                .SelectMany(x => x.Alternatives)
                .Select(x => new RecognitionAlternative(x.Transcript, x.Confidence, x.Words.ToRecognitionWords()));

            var sourceFileName = Path.GetFileName(transcribeAudioFile.Path);
            var dateCreated = DateTime.UtcNow;
            var transcribeItem = new TranscribeItem
            {
                Id = transcribeAudioFile.Id,
                AudioFileId = transcribeAudioFile.AudioFileId,
                ApplicationId = _appSettings.ApplicationId,
                Alternatives = JsonConvert.SerializeObject(alternatives),
                SourceFileName = sourceFileName,
                Storage = StorageSetting.Azure,
                StartTime = transcribeAudioFile.StartTime,
                EndTime = transcribeAudioFile.EndTime,
                TotalTime = transcribeAudioFile.TotalTime,
                DateCreatedUtc = dateCreated,
                DateUpdatedUtc = dateCreated
            };

            _logger.Information($"Audio file '{transcribeAudioFile.Path}' was recognized");

            return transcribeItem;
        }

        private SpeechClient CreateSpeechClient()
        {
            _logger.Information("Create speech recognition client");

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