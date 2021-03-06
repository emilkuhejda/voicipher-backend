﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Business.Extensions;
using Voicipher.Business.Utils;
using Voicipher.Common.Utils;
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
using Voicipher.Domain.State;
using Voicipher.Domain.Utils;

namespace Voicipher.Business.StateMachine
{
    public class JobStateMachine : IJobStateMachine
    {
        private readonly ICanRunRecognitionCommand _canRunRecognitionCommand;
        private readonly IModifySubscriptionTimeCommand _modifySubscriptionTimeCommand;
        private readonly IUpdateRecognitionStateCommand _updateRecognitionStateCommand;
        private readonly IWavFileService _wavFileService;
        private readonly ISpeechRecognitionService _speechRecognitionService;
        private readonly IMessageCenterService _messageCenterService;
        private readonly IBlobStorage _blobStorage;
        private readonly IFileAccessService _fileAccessService;
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly ITranscribeItemRepository _transcribeItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        private readonly MachineState _machineState = new();

        private BackgroundJob _backgroundJob;
        private AudioFile _audioFile;

        public JobStateMachine(
            ICanRunRecognitionCommand canRunRecognitionCommand,
            IModifySubscriptionTimeCommand modifySubscriptionTimeCommand,
            IUpdateRecognitionStateCommand updateRecognitionStateCommand,
            IWavFileService wavFileService,
            ISpeechRecognitionService speechRecognitionService,
            IMessageCenterService messageCenterService,
            IBlobStorage blobStorage,
            IFileAccessService fileAccessService,
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
            _messageCenterService = messageCenterService;
            _blobStorage = blobStorage;
            _fileAccessService = fileAccessService;
            _audioFileRepository = audioFileRepository;
            _transcribeItemRepository = transcribeItemRepository;
            _unitOfWork = unitOfWork;
            _appSettings = options.Value;
            _logger = logger.ForContext<JobStateMachine>();
        }

        private JobState CurrentState => _machineState.JobState;

        public void DoInit(BackgroundJob backgroundJob)
        {
            _backgroundJob = backgroundJob;
            _machineState.FromBackgroundJob(backgroundJob);

            TryChangeState(JobState.Initialized);
        }

        public async Task DoValidationAsync(CancellationToken cancellationToken)
        {
            TryChangeState(JobState.Validating);

            _audioFile = await _audioFileRepository.GetAsync(_machineState.UserId, _machineState.AudioFileId, cancellationToken);
            if (_audioFile == null)
                throw new FileNotFoundException($"Audio file {_machineState.AudioFileId} not found");

            var canRunRecognitionResult = await _canRunRecognitionCommand.ExecuteAsync(new CanRunRecognitionPayload(_machineState.UserId), null, cancellationToken);
            if (!canRunRecognitionResult.IsSuccess)
                throw new InvalidOperationException($"User ID {_machineState.UserId} does not have enough free minutes in the subscription");

            TryChangeState(JobState.Validated);
        }

        public async Task DoConvertingAsync(CancellationToken cancellationToken)
        {
            TryChangeState(JobState.Converting);
            await _wavFileService.RunConversionToWavAsync(_audioFile, cancellationToken);
            TryChangeState(JobState.Converted);
        }

        public async Task DoSplitAsync(CancellationToken cancellationToken)
        {
            TryChangeState(JobState.Splitting);

            var transcribedAudioFiles = await _wavFileService.SplitAudioFileAsync(_audioFile, cancellationToken);
            _machineState.TranscribedAudioFiles = transcribedAudioFiles;

            _logger.Information($"[{_audioFile.UserId}] Audio file was split to {transcribedAudioFiles.Length} partial audio files");

            _logger.Verbose($"[{_audioFile.UserId}] Start deleting transcribed items {_audioFile.OriginalSourceFileName} from blob storage");
            var blobSettings = new DeleteBlobSettings(_audioFile.OriginalSourceFileName, _audioFile.UserId, _audioFile.Id);
            await _blobStorage.DeleteTranscribedFiles(blobSettings, cancellationToken);
            _logger.Verbose($"[{_audioFile.UserId}] Transcribed items {_audioFile.OriginalSourceFileName} was deleted from blob storage");

            _logger.Verbose($"[{_audioFile.UserId}] {transcribedAudioFiles.Length} transcription items ready for upload");

            if (transcribedAudioFiles.Any())
            {
                foreach (var transcribedAudioFile in transcribedAudioFiles)
                {
                    if (!_fileAccessService.Exists(transcribedAudioFile.Path))
                        throw new FileNotFoundException($"[{_audioFile.UserId}] Transcribed audio file {transcribedAudioFile.Path} is not found");

                    _logger.Verbose($"[{_audioFile.UserId}] Start uploading transcription audio file {transcribedAudioFile.SourceFileName} to blob storage");

                    var metadata = new Dictionary<string, string> { { BlobMetadata.TranscribedAudioFile, true.ToString() } };
                    var uploadBlobSettings = new UploadBlobSettings(transcribedAudioFile.Path, _audioFile.UserId, _audioFile.Id, transcribedAudioFile.SourceFileName, metadata);
                    await _blobStorage.UploadAsync(uploadBlobSettings, cancellationToken);

                    _logger.Verbose($"[{_audioFile.UserId}] Transcription audio file {transcribedAudioFile.SourceFileName} was uploaded to blob storage");

                    _fileAccessService.Delete(transcribedAudioFile.Path);
                }

                _logger.Information($"[{_audioFile.UserId}] Audio files ({transcribedAudioFiles.Length}) were uploaded to blob storage and delete from temporary storage");
            }

            TryChangeState(JobState.Splitted);
        }

        public async Task DoProcessingAsync(CancellationToken cancellationToken)
        {
            TryChangeState(JobState.Processing);

            var transcribedAudioFiles = _machineState.TranscribedAudioFiles.OrderByDescending(x => x.EndTime).ToArray();
            var transcribedTime = transcribedAudioFiles.FirstOrDefault()?.EndTime ?? TimeSpan.Zero;
            _audioFile.TranscribedTime = transcribedTime;
            await _unitOfWork.SaveAsync(cancellationToken);
            _logger.Information($"[{_audioFile.UserId}] Transcribed time audio file {_audioFile.Id} was updated to {transcribedTime}");

            _logger.Information($"[{_audioFile.UserId}] Start speech recognition for audio file {_audioFile.Id}");
            var transcribeItems = await _speechRecognitionService.RecognizeAsync(_audioFile, transcribedAudioFiles, cancellationToken);
            _logger.Information($"[{_audioFile.UserId}] Speech recognition for audio file {_audioFile.Id} is finished");

            await _transcribeItemRepository.AddRangeAsync(transcribeItems, cancellationToken);
            await _transcribeItemRepository.SaveAsync(cancellationToken);

            TryChangeState(JobState.Processed);
        }

        public async Task DoCompleteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _machineState.DateCompletedUtc = DateTime.UtcNow;

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

                _audioFile.SourceFileName = string.Empty;

                TryChangeState(JobState.Completed);
            }
            catch (RequestFailedException ex)
            {
                _logger.Error(ex, $"[{_audioFile.UserId}] Blob storage is unavailable");
                throw;
            }
        }

        public async Task DoErrorAsync(Exception exception, CancellationToken cancellationToken)
        {
            _backgroundJob.Exception = ExceptionFormatter.FormatException(exception);
            await _unitOfWork.SaveAsync(cancellationToken);

            var updateRecognitionStatePayload = new UpdateRecognitionStatePayload(_machineState.AudioFileId, _machineState.UserId, _appSettings.ApplicationId, RecognitionState.None);
            await _updateRecognitionStateCommand.ExecuteAsync(updateRecognitionStatePayload, null, cancellationToken);

            await _messageCenterService.SendAsync(HubMethodsHelper.GetRecognitionErrorMethod(_machineState.UserId), _machineState.FileName);
        }

        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            _backgroundJob.FromState(_machineState);

            await _unitOfWork.SaveAsync(cancellationToken);
        }

        public void DoClean()
        {
            foreach (var transcribedAudioFile in _machineState.TranscribedAudioFiles)
            {
                _fileAccessService.Delete(transcribedAudioFile.Path);
            }
        }

        private void TryChangeState(JobState jobState)
        {
            if (CanTransition(CurrentState) != jobState)
                throw new InvalidOperationException($"Invalid transition operation from {CurrentState} to {jobState}");

            _machineState.JobState = jobState;
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
                    return JobState.Splitting;
                case JobState.Splitting:
                    return JobState.Splitted;
                case JobState.Splitted:
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
