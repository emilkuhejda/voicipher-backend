using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Exceptions;
using Voicipher.Domain.Interfaces.Commands;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Interfaces.StateMachine;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads;
using Voicipher.Domain.Payloads.Transcription;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.StateMachine
{
    public class JobStateMachine : IJobStateMachine
    {
        private readonly ICanRunRecognitionCommand _canRunRecognitionCommand;
        private readonly IModifySubscriptionTimeCommand _modifySubscriptionTimeCommand;
        private readonly IUpdateRecognitionStateCommand _updateRecognitionStateCommand;
        private readonly IWavFileService _wavFileService;
        private readonly ISpeechRecognitionService _speechRecognitionService;
        private readonly IBlobStorage _blobStorage;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly ITranscribeItemRepository _transcribeItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        private BackgroundJob _backgroundJob;
        private AudioFile _audioFile;
        private Dictionary<BackgroundJobParameter, object> _backgroundJobParameter = new();

        public JobStateMachine(
            ICanRunRecognitionCommand canRunRecognitionCommand,
            IModifySubscriptionTimeCommand modifySubscriptionTimeCommand,
            IUpdateRecognitionStateCommand updateRecognitionStateCommand,
            IWavFileService wavFileService,
            ISpeechRecognitionService speechRecognitionService,
            IBlobStorage blobStorage,
            IAudioFileRepository audioFileRepository,
            ITranscribeItemRepository transcribeItemRepository,
            IUnitOfWork unitOfWork,
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _canRunRecognitionCommand = canRunRecognitionCommand;
            _modifySubscriptionTimeCommand = modifySubscriptionTimeCommand;
            _updateRecognitionStateCommand = updateRecognitionStateCommand;
            _wavFileService = wavFileService;
            _speechRecognitionService = speechRecognitionService;
            _blobStorage = blobStorage;
            _audioFileRepository = audioFileRepository;
            _transcribeItemRepository = transcribeItemRepository;
            _unitOfWork = unitOfWork;
            _appSettings = options.Value;
            _logger = logger.ForContext<JobStateMachine>();
        }

        private JobState CurrentState => _backgroundJob.JobState;

        public void DoInit(BackgroundJob backgroundJob)
        {
            _backgroundJob = backgroundJob;
            _backgroundJob.Attempt += 1;
            _backgroundJob.JobState = JobState.Idle;

            _backgroundJobParameter = JsonConvert.DeserializeObject<Dictionary<BackgroundJobParameter, object>>(backgroundJob.Parameters);

            TryChangeState(JobState.Initialized);
        }

        public async Task DoValidationAsync(CancellationToken cancellationToken)
        {
            TryChangeState(JobState.Validating);

            _audioFile = await _audioFileRepository.GetAsync(_backgroundJob.UserId, _backgroundJob.AudioFileId, cancellationToken);
            if (_audioFile == null)
                throw new FileNotFoundException($"Audio file {_backgroundJob.AudioFileId} not found");

            var canRunRecognitionResult = await _canRunRecognitionCommand.ExecuteAsync(new CanRunRecognitionPayload(_backgroundJob.UserId), null, cancellationToken);
            if (!canRunRecognitionResult.IsSuccess)
                throw new InvalidOperationException($"User ID '{_backgroundJob.UserId}' does not have enough free minutes in the subscription");

            TryChangeState(JobState.Validated);
        }

        public async Task DoConvertingAsync(CancellationToken cancellationToken)
        {
            TryChangeState(JobState.Converting);
            await _wavFileService.RunConversionToWavAsync(_audioFile, cancellationToken);
            TryChangeState(JobState.Converted);
        }

        public async Task DoProcessingAsync(CancellationToken cancellationToken)
        {
            TryChangeState(JobState.Processing);

            var transcribeAudioFiles = _backgroundJobParameter.GetValue<TranscribeAudioFile[]>(BackgroundJobParameter.AudioFiles);
            if (transcribeAudioFiles == null || !transcribeAudioFiles.Any() || transcribeAudioFiles.Any(x => !File.Exists(x.Path)))
            {
                transcribeAudioFiles = await _wavFileService.SplitAudioFileAsync(_audioFile, cancellationToken);
                _backgroundJobParameter.AddOrUpdate(BackgroundJobParameter.AudioFiles, transcribeAudioFiles);

                _logger.Information($"Audio file was split to {transcribeAudioFiles.Length} partial audio files");
            }

            var transcribedTime = transcribeAudioFiles.OrderByDescending(x => x.EndTime).FirstOrDefault()?.EndTime ?? TimeSpan.Zero;
            _audioFile.TranscribedTime = transcribedTime;
            await _unitOfWork.SaveAsync(cancellationToken);
            _logger.Information($"Transcribed time audio file '{_audioFile.Id}' was updated to {transcribedTime}");

            var transcribeItems = await _speechRecognitionService.RecognizeAsync(transcribeAudioFiles, _audioFile.Language, cancellationToken);
            await _transcribeItemRepository.AddRangeAsync(transcribeItems, cancellationToken);
            await _transcribeItemRepository.SaveAsync(cancellationToken);

            TryChangeState(JobState.Processed);
        }

        public async Task DoCompleteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var transcribeAudioFiles = _backgroundJobParameter.GetValue<TranscribeAudioFile[]>(BackgroundJobParameter.AudioFiles);
                if (transcribeAudioFiles != null && transcribeAudioFiles.Any())
                {
                    foreach (var transcribeAudioFile in transcribeAudioFiles)
                    {
                        if (File.Exists(transcribeAudioFile.Path))
                        {
                            var uploadBlobSettings = new UploadBlobSettings(transcribeAudioFile.Path, _audioFile.UserId, _audioFile.Id, transcribeAudioFile.SourceFileName);
                            await _blobStorage.UploadAsync(uploadBlobSettings, cancellationToken);
                            File.Delete(transcribeAudioFile.Path);
                        }
                    }

                    _logger.Information($"Audio fIle ({transcribeAudioFiles.Length}) were uploaded to blob storage and delete from temporary storage");
                }

                _backgroundJob.DateCompletedUtc = DateTime.UtcNow;
                _backgroundJobParameter.Remove(BackgroundJobParameter.AudioFiles);

                var blobSettings = new DeleteBlobSettings(_audioFile.OriginalSourceFileName, _audioFile.UserId, _audioFile.Id);
                await _blobStorage.DeleteFileBlobAsync(blobSettings, cancellationToken);

                var modifySubscriptionTimePayload = new ModifySubscriptionTimePayload
                {
                    UserId = _audioFile.UserId,
                    ApplicationId = _appSettings.ApplicationId,
                    Time = _audioFile.TranscribedTime,
                    Operation = SubscriptionOperation.Remove
                };
                var modifySubscriptionCommandResult = await _modifySubscriptionTimeCommand.ExecuteAsync(modifySubscriptionTimePayload, null, cancellationToken);
                if (!modifySubscriptionCommandResult.IsSuccess)
                    throw new OperationErrorException(modifySubscriptionCommandResult.Error.ErrorCode);

                var updateRecognitionStatePayload = new UpdateRecognitionStatePayload(_audioFile.Id, _audioFile.UserId, _appSettings.ApplicationId, RecognitionState.Completed);
                await _updateRecognitionStateCommand.ExecuteAsync(updateRecognitionStatePayload, null, cancellationToken);

                _audioFile.OriginalSourceFileName = string.Empty;
                _audioFile.SourceFileName = string.Empty;

                TryChangeState(JobState.Completed);
            }
            catch (RequestFailedException ex)
            {
                _logger.Error(ex, "Blob storage is unavailable");
                throw;
            }
        }

        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            _backgroundJob.Parameters = JsonConvert.SerializeObject(_backgroundJobParameter);
            await _unitOfWork.SaveAsync(cancellationToken);
        }

        private void TryChangeState(JobState jobState)
        {
            if (CanTransition(CurrentState) != jobState)
                throw new InvalidOperationException($"Invalid transition operation from {CurrentState} to {jobState}");

            _backgroundJob.JobState = jobState;
        }

        private JobState CanTransition(JobState jobState)
        {
            switch (jobState)
            {
                case JobState.Idle:
                    return JobState.Initialized;
                case JobState.Initialized:
                    return JobState.Validating;
                case JobState.Validating:
                    return JobState.Validated;
                case JobState.Validated:
                    return JobState.Converting;
                case JobState.Converting:
                    return JobState.Converted;
                case JobState.Converted:
                    return JobState.Processing;
                case JobState.Processing:
                    return JobState.Processed;
                case JobState.Processed:
                    return JobState.Completed;
                default:
                    throw new InvalidEnumArgumentException(nameof(jobState));
            }
        }
    }
}
