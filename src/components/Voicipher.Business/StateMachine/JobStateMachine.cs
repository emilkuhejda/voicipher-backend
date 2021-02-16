using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.StateMachine;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads;

namespace Voicipher.Business.StateMachine
{
    public class JobStateMachine : IJobStateMachine
    {
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly ICanRunRecognitionCommand _canRunRecognitionCommand;
        private readonly IUnitOfWork _unitOfWork;

        private BackgroundJob _backgroundJob;

        public JobStateMachine(
            IAudioFileRepository audioFileRepository,
            ICanRunRecognitionCommand canRunRecognitionCommand,
            IUnitOfWork unitOfWork)
        {
            _audioFileRepository = audioFileRepository;
            _canRunRecognitionCommand = canRunRecognitionCommand;
            _unitOfWork = unitOfWork;
        }

        private JobState CurrentState => _backgroundJob.JobState;

        public void DoInit(BackgroundJob backgroundJob)
        {
            _backgroundJob = backgroundJob;
            _backgroundJob.JobState = JobState.Idle;

            TryChangeState(JobState.Initialized);
        }

        public async Task DoValidationAsync(CancellationToken cancellationToken)
        {
            TryChangeState(JobState.Validating);

            var audioFile = await _audioFileRepository.GetAsync(_backgroundJob.UserId, _backgroundJob.AudioFileId, cancellationToken);
            if (audioFile == null)
                throw new FileNotFoundException($"Audio file {_backgroundJob.AudioFileId} not found");

            var canRunRecognitionResult = await _canRunRecognitionCommand.ExecuteAsync(new CanRunRecognitionPayload(_backgroundJob.UserId), null, cancellationToken);
            if (!canRunRecognitionResult.IsSuccess)
                throw new InvalidOperationException($"User ID '{_backgroundJob.UserId}' does not have enough free minutes in the subscription");

            TryChangeState(JobState.Validated);
        }

        public async Task DoConvertingAsync(CancellationToken cancellationToken)
        {
            TryChangeState(JobState.Converting);
            await Task.CompletedTask;
            TryChangeState(JobState.Converted);
        }

        public async Task DoProcessingAsync(CancellationToken cancellationToken)
        {
            TryChangeState(JobState.Processing);
            await Task.CompletedTask;
            TryChangeState(JobState.Processed);
        }

        public async Task DoCompleteAsync(CancellationToken cancellationToken)
        {
            _backgroundJob.DateCompletedUtc = DateTime.UtcNow;

            TryChangeState(JobState.Completed);

            await _unitOfWork.SaveAsync(cancellationToken);
        }

        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            if (CurrentState == JobState.Completed)
                return;

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
