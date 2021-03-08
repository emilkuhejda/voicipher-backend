using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
        private readonly IFileAccessService _fileAccessService;
        private readonly IBlobStorage _blobStorage;
        private readonly IDiskStorage _diskStorage;
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
            IFileAccessService fileAccessService,
            IBlobStorage blobStorage,
            IIndex<StorageLocation, IDiskStorage> index,
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
            _fileAccessService = fileAccessService;
            _blobStorage = blobStorage;
            _diskStorage = index[StorageLocation.Audio];
            _audioFileRepository = audioFileRepository;
            _transcribeItemRepository = transcribeItemRepository;
            _unitOfWork = unitOfWork;
            _appSettings = options.Value;
            _logger = logger.ForContext<JobStateMachine>();
        }

        public IMachineState MachineState => _machineState;

        private JobState CurrentState => _machineState.JobState;

        public async Task DoInitAsync(BackgroundJob backgroundJob, CancellationToken cancellationToken)
        {
            _backgroundJob = backgroundJob;
            _machineState.StateFileName = $"{backgroundJob.AudioFileId}.json";
            _machineState.FromBackgroundJob(backgroundJob);

            var stateFilePath = Path.Combine(_diskStorage.GetDirectoryPath(), _machineState.StateFileName);
            if (_fileAccessService.Exists(stateFilePath))
            {
                var json = await _fileAccessService.ReadAllTextAsync(stateFilePath, cancellationToken);
                var stateToRestore = JsonConvert.DeserializeObject<IMachineState>(json);
                _machineState.FromState(stateToRestore);
            }

            await TryChangeStateAsync(JobState.Initialized, cancellationToken);
        }

        public async Task DoValidationAsync(CancellationToken cancellationToken)
        {
            await TryChangeStateAsync(JobState.Validating, cancellationToken);

            _audioFile = await _audioFileRepository.GetAsync(_machineState.UserId, _machineState.AudioFileId, cancellationToken);
            if (_audioFile == null)
                throw new FileNotFoundException($"Audio file {_machineState.AudioFileId} not found");

            var canRunRecognitionResult = await _canRunRecognitionCommand.ExecuteAsync(new CanRunRecognitionPayload(_machineState.UserId), null, cancellationToken);
            if (!canRunRecognitionResult.IsSuccess)
                throw new InvalidOperationException($"User ID {_machineState.UserId} does not have enough free minutes in the subscription");

            await TryChangeStateAsync(JobState.Validated, cancellationToken);
        }

        public async Task DoConvertingAsync(CancellationToken cancellationToken)
        {
            await TryChangeStateAsync(JobState.Converting, cancellationToken);
            if (CanSkip(JobState.Converting))
            {
                _audioFile.SourceFileName = _machineState.WavSourceFileName;
                return;
            }

            _logger.Verbose($"[{_audioFile.Id}] Remove temporary folder for audio file {_audioFile.Id}");
            _diskStorage.DeleteFolder(_audioFile.Id.ToString());

            var sourceFileName = await _wavFileService.RunConversionToWavAsync(_audioFile, cancellationToken);
            _audioFile.SourceFileName = sourceFileName;
            _machineState.WavSourceFileName = sourceFileName;

            await TryChangeStateAsync(JobState.Converted, cancellationToken);
        }

        public async Task DoSplitAsync(CancellationToken cancellationToken)
        {
            await TryChangeStateAsync(JobState.Splitting, cancellationToken);
            if (CanSkip(JobState.Splitting))
                return;

            if (!_machineState.TranscribedAudioFiles.Any() || _machineState.TranscribedAudioFiles.Any(x => !_fileAccessService.Exists(x.Path)))
            {
                _machineState.ClearTranscribedAudioFiles();
                var transcribedAudioFiles = await _wavFileService.SplitAudioFileAsync(_audioFile, cancellationToken);
                _machineState.TranscribedAudioFiles = transcribedAudioFiles;
                await SaveStateAsync(cancellationToken);

                _logger.Information($"[{_audioFile.UserId}] Audio file was split to {_machineState.TranscribedAudioFiles.Length} partial audio files");
            }

            _logger.Verbose($"[{_audioFile.UserId}] {_machineState.TranscribedAudioFiles.Length} transcription items ready for upload");

            foreach (var transcribedAudioFile in _machineState.TranscribedAudioFiles)
            {
                if (!_fileAccessService.Exists(transcribedAudioFile.Path))
                    throw new FileNotFoundException($"[{_audioFile.UserId}] Transcribed audio file {transcribedAudioFile.Path} is not found");

                _logger.Verbose($"[{_audioFile.UserId}] Start uploading transcription audio file {transcribedAudioFile.SourceFileName} to blob storage");

                var metadata = new Dictionary<string, string> { { BlobMetadata.TranscribedAudioFile, true.ToString() } };
                var uploadBlobSettings = new UploadBlobSettings(transcribedAudioFile.Path, _audioFile.UserId, _audioFile.Id, transcribedAudioFile.SourceFileName, metadata);
                await _blobStorage.UploadAsync(uploadBlobSettings, cancellationToken);

                _logger.Verbose($"[{_audioFile.UserId}] Transcription audio file {transcribedAudioFile.SourceFileName} was uploaded to blob storage");
            }

            _logger.Information($"[{_audioFile.UserId}] Audio files ({_machineState.TranscribedAudioFiles.Length}) were uploaded to blob storage");

            var diskStorageSettings = new DiskStorageSettings(_machineState.FolderName, _machineState.WavSourceFileName);
            _diskStorage.Delete(diskStorageSettings);
            _logger.Verbose($"[{_machineState.AudioFileId}] Wav audio file {_machineState.WavSourceFileName} was deleted from temporary disk storage");

            await TryChangeStateAsync(JobState.Splitted, cancellationToken);
        }

        public async Task DoProcessingAsync(CancellationToken cancellationToken)
        {
            await TryChangeStateAsync(JobState.Processing, cancellationToken);

            var transcribedAudioFiles = _machineState.TranscribedAudioFiles.OrderByDescending(x => x.EndTime).ToArray();
            var transcribedTime = transcribedAudioFiles.FirstOrDefault()?.EndTime ?? TimeSpan.Zero;
            _audioFile.TranscribedTime = transcribedTime;
            await _unitOfWork.SaveAsync(cancellationToken);
            _logger.Information($"[{_audioFile.UserId}] Transcribed time audio file {_audioFile.Id} was updated to {transcribedTime}");

            _logger.Information($"[{_audioFile.UserId}] Start speech recognition for audio file {_audioFile.Id}");
            var transcribeItems = await _speechRecognitionService.RecognizeAsync(_audioFile, transcribedAudioFiles, cancellationToken);
            _logger.Information($"[{_audioFile.UserId}] Speech recognition for audio file {_audioFile.Id} is finished");

            _transcribeItemRepository.RemoveRange(_machineState.AudioFileId);
            await _transcribeItemRepository.AddRangeAsync(transcribeItems, cancellationToken);
            await _transcribeItemRepository.SaveAsync(cancellationToken);

            await TryChangeStateAsync(JobState.Processed, cancellationToken);
        }

        public async Task DoCompleteAsync(CancellationToken cancellationToken)
        {
            await TryChangeStateAsync(JobState.Completing, cancellationToken);

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
            _audioFile.DateProcessedUtc = DateTime.UtcNow;

            await TryChangeStateAsync(JobState.Completed, cancellationToken);
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
            try
            {
                _logger.Information($"[{_machineState.AudioFileId}] Clean temporary data from disk storage");

                var diskStorageSettings = new DiskStorageSettings(_machineState.StateFileName);
                _diskStorage.Delete(diskStorageSettings);
                _diskStorage.DeleteFolder(_machineState.FolderName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{_machineState.UserId}] Clean up failed");
            }
        }

        private async Task TryChangeStateAsync(JobState jobState, CancellationToken cancellationToken)
        {
            if (_machineState.IsRestored && jobState <= CurrentState)
                return;

            if (CanTransition(CurrentState) != jobState)
                throw new InvalidOperationException($"Invalid transition operation from {CurrentState} to {jobState}");

            _machineState.JobState = jobState;

            await SaveStateAsync(cancellationToken);
        }

        private Task SaveStateAsync(CancellationToken cancellationToken)
        {
            var machineStateJson = JsonConvert.SerializeObject(_machineState);
            var diskStorageSettings = new DiskStorageSettings(_machineState.StateFileName);
            return _diskStorage.UploadAsync(Encoding.UTF8.GetBytes(machineStateJson), diskStorageSettings, cancellationToken);
        }

        private bool CanSkip(JobState jobState)
        {
            return _machineState.IsRestored && jobState < CurrentState;
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
                    return JobState.Completing;
                case JobState.Completing:
                    return JobState.Completed;
                default:
                    throw new InvalidEnumArgumentException(nameof(jobState));
            }
        }
    }
}
