using System;
using System.ComponentModel;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Interfaces.StateMachine;
using Voicipher.Domain.Models;

namespace Voicipher.Business.StateMachine
{
    public class JobStateMachine : IJobStateMachine
    {
        private JobState _restoredJobState;
        private BackgroundJob _backgroundJob;

        public JobStateMachine()
        {
        }

        private JobState CurrentState => _backgroundJob.JobState;

        public void DoInit(BackgroundJob backgroundJob)
        {
            _backgroundJob = backgroundJob;
            _restoredJobState = backgroundJob.JobState;
            _backgroundJob.JobState = JobState.Idle;

            TryChangeState(JobState.Initialized);
        }

        public void DoValidation()
        {
            TryChangeState(JobState.Validating);
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
