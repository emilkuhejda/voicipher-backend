using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.DataAccess;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Transcription;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.StateMachine;
using Voicipher.Domain.Payloads.Job;
using Voicipher.Domain.Payloads.Transcription;
using Voicipher.Domain.Settings;

namespace Voicipher.Business.Commands.Job
{
    public class RunBackgroundJobCommand : Command<BackgroundJobPayload, CommandResult>, IRunBackgroundJobCommand
    {
        private readonly IUpdateRecognitionStateCommand _updateRecognitionStateCommand;
        private readonly IJobStateMachine _jobStateMachine;
        private readonly IBackgroundJobRepository _backgroundJobRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public RunBackgroundJobCommand(
            IUpdateRecognitionStateCommand updateRecognitionStateCommand,
            IJobStateMachine jobStateMachine,
            IBackgroundJobRepository backgroundJobRepository,
            IUnitOfWork unitOfWork,
            IOptions<AppSettings> options,
            ILogger logger)
        {
            _updateRecognitionStateCommand = updateRecognitionStateCommand;
            _jobStateMachine = jobStateMachine;
            _backgroundJobRepository = backgroundJobRepository;
            _unitOfWork = unitOfWork;
            _appSettings = options.Value;
            _logger = logger.ForContext<RunBackgroundJobCommand>();
        }

        protected override async Task<CommandResult> Execute(BackgroundJobPayload parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            _logger.Information($"Background job {parameter.Id} has started");

            using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var backgroundJob = await _backgroundJobRepository.GetAsync(parameter.Id, cancellationToken);
                    if (backgroundJob == null)
                        throw new InvalidOperationException($"Background job {parameter.Id} not found");

                    _jobStateMachine.DoInit(backgroundJob);
                    await _jobStateMachine.DoValidationAsync(cancellationToken);
                    await _jobStateMachine.DoConvertingAsync(cancellationToken);
                    await _jobStateMachine.DoProcessingAsync(cancellationToken);
                    await _jobStateMachine.DoCompleteAsync(cancellationToken);

                    _logger.Information($"Background job {parameter.Id} is completed");

                    var payload = new UpdateRecognitionStatePayload(parameter.AudioFileId, parameter.UserId, _appSettings.ApplicationId, RecognitionState.Completed);
                    await _updateRecognitionStateCommand.ExecuteAsync(payload, null, cancellationToken);

                    return new CommandResult();
                }
                finally
                {
                    await _jobStateMachine.SaveAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
            }
        }
    }
}
