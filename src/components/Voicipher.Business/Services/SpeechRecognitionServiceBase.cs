using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
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
using Voicipher.Domain.Transcription;
using Voicipher.Domain.Utils;

namespace Voicipher.Business.Services
{
    public abstract class SpeechRecognitionServiceBase : ISpeechRecognitionService
    {
        private readonly IAudioFileProcessingChannel _audioFileProcessingChannel;
        private readonly IMessageCenterService _messageCenterService;
        private readonly IDiskStorage _diskStorage;
        private readonly AppSettings _appSettings;

        private int _totalTasks;
        private int _tasksDone;

        protected SpeechRecognitionServiceBase(
            IAudioFileProcessingChannel audioFileProcessingChannel,
            IMessageCenterService messageCenterService,
            IIndex<StorageLocation, IDiskStorage> index,
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _audioFileProcessingChannel = audioFileProcessingChannel;
            _messageCenterService = messageCenterService;
            _diskStorage = index[StorageLocation.Audio];
            _appSettings = options.Value;
            Logger = logger.ForContext<SpeechRecognitionService>();
        }

        protected ILogger Logger { get; }

        public bool CanCreateSpeechClientAsync()
        {
            try
            {
                var speechClient = CreateSpeechClient();
                return speechClient != null;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, $"Unable to create speech recognition client");
            }

            return false;
        }

        public async Task<TranscribeItem[]> RecognizeAsync(AudioFile audioFile, TranscribedAudioFile[] transcribedAudioFiles, CancellationToken cancellationToken)
        {
            var speechRecognizeConfig = new SpeechRecognizeConfig(audioFile);

            var updateMethods = new List<Func<Task<TranscribeItem>>>();
            foreach (var transcribeAudioFile in transcribedAudioFiles)
            {
                updateMethods.Add(() => RecognizeSpeech(transcribeAudioFile, speechRecognizeConfig, cancellationToken));
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

            if (transcribeItems.Any() && transcribeItems.All(x => x.IsIncomplete))
            {
                throw new InvalidOperationException($"[{audioFile.UserId}] Speech recognition operation failed");
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

        private async Task<TranscribeItem> RecognizeSpeech(TranscribedAudioFile transcribedAudioFile, SpeechRecognizeConfig speechRecognizeConfig, CancellationToken cancellationToken)
        {
            var speechClient = CreateSpeechClient();

            Logger.Verbose($"[{speechRecognizeConfig.UserId}] Start speech recognition for file {transcribedAudioFile.Path}");

            var recognizedResult = await GetRecognizedResultAsync(speechClient, transcribedAudioFile, speechRecognizeConfig);

            var fileName = $"{transcribedAudioFile.Id}.json";
            var recognizedResultJson = JsonConvert.SerializeObject(recognizedResult);
            var diskStorageSettings = new DiskStorageSettings(speechRecognizeConfig.AudioFileId.ToString(), fileName);
            var filePath = await _diskStorage.UploadAsync(Encoding.UTF8.GetBytes(recognizedResultJson), diskStorageSettings, cancellationToken);
            Logger.Information($"[{speechRecognizeConfig.UserId}] Store recognition result to disk in destination {filePath}");

            var dateCreated = DateTime.UtcNow;
            var transcribeItem = new TranscribeItem
            {
                Id = transcribedAudioFile.Id,
                AudioFileId = transcribedAudioFile.AudioFileId,
                ApplicationId = _appSettings.ApplicationId,
                Alternatives = JsonConvert.SerializeObject(recognizedResult.Alternatives),
                SourceFileName = transcribedAudioFile.SourceFileName,
                StartTime = transcribedAudioFile.StartTime,
                EndTime = transcribedAudioFile.EndTime,
                TotalTime = transcribedAudioFile.TotalTime,
                IsIncomplete = recognizedResult.IsIncomplete,
                DateCreatedUtc = dateCreated,
                DateUpdatedUtc = dateCreated
            };

            Logger.Verbose($"[{speechRecognizeConfig.UserId}] Audio file {transcribedAudioFile.Path} was recognized");

            return transcribeItem;
        }

        protected abstract Task<RecognizedResult> GetRecognizedResultAsync(SpeechClient speechClient, TranscribedAudioFile transcribedAudioFile, SpeechRecognizeConfig speechRecognizeConfig);

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
