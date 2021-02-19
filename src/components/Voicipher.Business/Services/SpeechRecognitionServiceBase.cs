using System;
using System.Collections.Generic;
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
using Voicipher.Business.Utils;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Services
{
    public abstract class SpeechRecognitionServiceBase : ISpeechRecognitionService
    {
        private readonly IAudioFileProcessingChannel _audioFileProcessingChannel;
        private readonly IMessageCenterService _messageCenterService;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        private int _totalTasks;
        private int _tasksDone;

        protected SpeechRecognitionServiceBase(
            IAudioFileProcessingChannel audioFileProcessingChannel,
            IMessageCenterService messageCenterService,
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _audioFileProcessingChannel = audioFileProcessingChannel;
            _messageCenterService = messageCenterService;
            _appSettings = options.Value;
            _logger = logger.ForContext<SpeechRecognitionService>();
        }

        public bool CanCreateSpeechClientAsync()
        {
            try
            {
                var speechClient = CreateSpeechClient();
                return speechClient != null;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, $"Unable to create speech recognition client");
            }

            return false;
        }

        public async Task<TranscribeItem[]> RecognizeAsync(AudioFile audioFile, TranscribedAudioFile[] transcribedAudioFiles, CancellationToken cancellationToken)
        {
            var speechClient = CreateSpeechClient();

            _logger.Information($"[{audioFile.UserId}] Speech recognition client was created");

            var updateMethods = new List<Func<Task<TranscribeItem>>>();
            foreach (var transcribeAudioFile in transcribedAudioFiles)
            {
                updateMethods.Add(() => RecognizeSpeech(speechClient, transcribeAudioFile, audioFile.Language, audioFile.UserId));
            }

            _totalTasks = updateMethods.Count;
            _tasksDone = 0;

            var transcribeItems = new List<TranscribeItem>();
            foreach (var enumerable in updateMethods.Split(10))
            {
                var tasks = enumerable.WhenTaskDone(async () => await UpdateProgressAsync(audioFile.Id, audioFile.UserId)).Select(x => x());
                var items = await Task.WhenAll(tasks);
                transcribeItems.AddRange(items);
            }

            return transcribeItems.ToArray();
        }

        private async Task UpdateProgressAsync(Guid audioFileId, Guid userId)
        {
            var currentTask = Interlocked.Increment(ref _tasksDone);
            var percentageDone = (int)((double)currentTask / _totalTasks * 100);
            _audioFileProcessingChannel.UpdateProgress(audioFileId, percentageDone);

            var outputModel = new CacheItemOutputModel(audioFileId, RecognitionState.InProgress, percentageDone);
            await _messageCenterService.SendAsync(HubMethodsHelper.GetRecognitionProgressChangedMethod(userId), outputModel);
        }

        private async Task<TranscribeItem> RecognizeSpeech(SpeechClient speech, TranscribedAudioFile transcribedAudioFile, string language, Guid userId)
        {
            _logger.Information($"[{userId}] Start speech recognition for file {transcribedAudioFile.Path}");

            var response = await GetRecognizedResponseAsync(speech, transcribedAudioFile, language);
            var alternatives = response.Results
                .SelectMany(x => x.Alternatives)
                .Select(x => new RecognitionAlternative(x.Transcript, x.Confidence, x.Words.ToRecognitionWords()));

            var dateCreated = DateTime.UtcNow;
            var transcribeItem = new TranscribeItem
            {
                Id = transcribedAudioFile.Id,
                AudioFileId = transcribedAudioFile.AudioFileId,
                ApplicationId = _appSettings.ApplicationId,
                Alternatives = JsonConvert.SerializeObject(alternatives),
                SourceFileName = transcribedAudioFile.SourceFileName,
                Storage = StorageSetting.Azure,
                StartTime = transcribedAudioFile.StartTime,
                EndTime = transcribedAudioFile.EndTime,
                TotalTime = transcribedAudioFile.TotalTime,
                DateCreatedUtc = dateCreated,
                DateUpdatedUtc = dateCreated
            };

            _logger.Information($"[{userId}] Audio file {transcribedAudioFile.Path} was recognized");

            return transcribeItem;
        }

        protected abstract Task<LongRunningRecognizeResponse> GetRecognizedResponseAsync(SpeechClient speech, TranscribedAudioFile transcribedAudioFile, string language);

        private SpeechClient CreateSpeechClient()
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
