using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.DataAccess;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.StateMachine;
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Domain.Payloads.Job;

namespace Voicipher.Business.Commands.Job
{
    public class RunBackgroundJobCommand : Command<BackgroundJobPayload, CommandResult>, IRunBackgroundJobCommand
    {
        private readonly IDeleteAudioFileSourceCommand _deleteAudioFileSourceCommand;
        private readonly IJobStateMachine _jobStateMachine;
        private readonly IBackgroundJobRepository _backgroundJobRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RunBackgroundJobCommand(
            IDeleteAudioFileSourceCommand deleteAudioFileSourceCommand,
            IJobStateMachine jobStateMachine,
            IBackgroundJobRepository backgroundJobRepository,
            IUnitOfWork unitOfWork,
            ILogger logger)
        {
            _deleteAudioFileSourceCommand = deleteAudioFileSourceCommand;
            _jobStateMachine = jobStateMachine;
            _backgroundJobRepository = backgroundJobRepository;
            _unitOfWork = unitOfWork;
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
                }
                catch (Exception ex)
                {
                    await _jobStateMachine.DoErrorAsync(ex, cancellationToken);

                    throw;
                }
                finally
                {
                    await _jobStateMachine.SaveAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    _jobStateMachine.DoClean();
                }

                _logger.Error($"Background job {parameter.Id} is completed");
            }

            var payload = new DeleteAudioFileSourcePayload(parameter.AudioFileId, parameter.UserId);
            var commandResult = await _deleteAudioFileSourceCommand.ExecuteAsync(payload, principal, cancellationToken);
            if (!commandResult.IsSuccess)
            {
                _logger.Error($"Delete audio source command failed with error code {commandResult.Error.ErrorCode}");
            }

            _logger.Information($"Background job {parameter.Id} is completed");

            return new CommandResult();
        }
    }
}
