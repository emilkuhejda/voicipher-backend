﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Google.Cloud.Speech.V1;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Utils;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;
using Voicipher.Domain.Transcription;
using Voicipher.Domain.Utils;

namespace Voicipher.Business.Services
{
    public abstract class SpeechRecognitionServiceBase : ISpeechRecognitionService
    {
        private readonly ISpeechClientFactory _speechClientFactory;
        private readonly IAudioFileProcessingChannel _audioFileProcessingChannel;
        private readonly IMessageCenterService _messageCenterService;
        private readonly IFileAccessService _fileAccessService;
        private readonly IDiskStorage _diskStorage;

        private int _totalTasks;
        private int _tasksDone;

        protected SpeechRecognitionServiceBase(
            ISpeechClientFactory speechClientFactory,
            IAudioFileProcessingChannel audioFileProcessingChannel,
            IMessageCenterService messageCenterService,
            IFileAccessService fileAccessService,
            IIndex<StorageLocation, IDiskStorage> index,
            ILogger logger)
        {
            _speechClientFactory = speechClientFactory;
            _audioFileProcessingChannel = audioFileProcessingChannel;
            _messageCenterService = messageCenterService;
            _fileAccessService = fileAccessService;
            _diskStorage = index[StorageLocation.Audio];
            Logger = logger.ForContext<SpeechRecognitionService>();
        }

        protected ILogger Logger { get; }

        public bool CanCreateSpeechClientAsync()
        {
            try
            {
                var speechClient = _speechClientFactory.CreateClient();
                return speechClient != null;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, $"Unable to create speech recognition client");
            }

            return false;
        }

        public async Task<TranscribeItem[]> RecognizeAsync(AudioFile audioFile, TranscribedAudioFile[] transcribedAudioFiles, Guid applicationId, CancellationToken cancellationToken)
        {
            var speechRecognizeConfig = new SpeechRecognizeConfig(audioFile, applicationId);

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
            var speechClient = _speechClientFactory.CreateClient();

            Logger.Verbose($"[{speechRecognizeConfig.UserId}] Start speech recognition for file {transcribedAudioFile.Path}");

            RecognizedResult recognizedResult;
            var fileName = $"{transcribedAudioFile.Id}.json";
            var filePath = GetFilePath(fileName, speechRecognizeConfig.AudioFileId);
            if (_fileAccessService.Exists(filePath))
            {
                var serializedRecognizedResult = await _fileAccessService.ReadAllTextAsync(filePath, cancellationToken);
                recognizedResult = JsonConvert.DeserializeObject<RecognizedResult>(serializedRecognizedResult);

                Logger.Verbose($"[{speechRecognizeConfig.UserId}] Recognition result restored from destination {filePath}");
            }
            else
            {
                recognizedResult = await GetRecognizedResultAsync(speechClient, transcribedAudioFile, speechRecognizeConfig);

                var serializedRecognizedResult = JsonConvert.SerializeObject(recognizedResult);
                var diskStorageSettings = new DiskStorageSettings(speechRecognizeConfig.AudioFileId.ToString(), fileName);
                filePath = await _diskStorage.UploadAsync(Encoding.UTF8.GetBytes(serializedRecognizedResult), diskStorageSettings, cancellationToken);
                Logger.Verbose($"[{speechRecognizeConfig.UserId}] Store recognition result to disk in destination {filePath}");
            }

            var dateCreated = DateTime.UtcNow;
            var transcribeItem = new TranscribeItem
            {
                Id = transcribedAudioFile.Id,
                AudioFileId = transcribedAudioFile.AudioFileId,
                ApplicationId = speechRecognizeConfig.ApplicationId,
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

        private string GetFilePath(string fileName, Guid audioFileId)
        {
            return Path.Combine(_diskStorage.GetDirectoryPath(audioFileId.ToString()), fileName);
        }
    }
}
