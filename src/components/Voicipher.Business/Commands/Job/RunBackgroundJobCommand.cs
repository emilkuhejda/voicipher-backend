using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Business.Infrastructure;
using Voicipher.Business.Utils;
using Voicipher.DataAccess;
using Voicipher.Domain.Configuration;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.Interfaces.Commands.Audio;
using Voicipher.Domain.Interfaces.Commands.Notifications;
using Voicipher.Domain.Interfaces.Queries.ControlPanel;
using Voicipher.Domain.Interfaces.Repositories;
using Voicipher.Domain.Interfaces.Services;
using Voicipher.Domain.Interfaces.StateMachine;
using Voicipher.Domain.Payloads.Audio;
using Voicipher.Domain.Payloads.Job;

namespace Voicipher.Business.Commands.Job
{
    public class RunBackgroundJobCommand : Command<BackgroundJobPayload, CommandResult>, IRunBackgroundJobCommand
    {
        private readonly ICreateInformationMessageCommand _createInformationMessageCommand;
        private readonly IDeleteAudioFileSourceCommand _deleteAudioFileSourceCommand;
        private readonly IGetInternalValueQuery<bool> _getInternalValueQuery;
        private readonly INotificationService _notificationService;
        private readonly IJobStateMachine _jobStateMachine;
        private readonly IBackgroundJobRepository _backgroundJobRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RunBackgroundJobCommand(
            ICreateInformationMessageCommand createInformationMessageCommand,
            IDeleteAudioFileSourceCommand deleteAudioFileSourceCommand,
            IGetInternalValueQuery<bool> getInternalValueQuery,
            INotificationService notificationService,
            IJobStateMachine jobStateMachine,
            IBackgroundJobRepository backgroundJobRepository,
            IUnitOfWork unitOfWork,
            ILogger logger)
        {
            _createInformationMessageCommand = createInformationMessageCommand;
            _deleteAudioFileSourceCommand = deleteAudioFileSourceCommand;
            _getInternalValueQuery = getInternalValueQuery;
            _notificationService = notificationService;
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

                var queryResult = await _getInternalValueQuery.ExecuteAsync(InternalValues.IsProgressNotificationsEnabled, null, cancellationToken);
                if (queryResult.IsSuccess && queryResult.Value.Value)
                {
                    await CreateAndSendNotifications(parameter, cancellationToken);
                }

                _logger.Verbose($"[{parameter.UserId}] Job state machine ended");
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

        private async Task CreateAndSendNotifications(BackgroundJobPayload parameter, CancellationToken cancellationToken)
        {
            try
            {
                var informationMessagePayload = GenericNotifications.GetTranscriptionSuccess(parameter.UserId, parameter.AudioFileId);
                var commandResult = await _createInformationMessageCommand.ExecuteAsync(informationMessagePayload, null, cancellationToken);
                if (!commandResult.IsSuccess)
                    return;

                _logger.Verbose($"[{parameter.UserId}] Start sending notification to devices after speech recognition of the audio file {parameter.AudioFileId}");
                var notificationResults = await _notificationService.SendAsync(commandResult.Value.Id, parameter.UserId, cancellationToken);
                _logger.Information($"[{parameter.UserId}] Send notification completed with result {JsonConvert.SerializeObject(notificationResults)}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Send notification failed");
            }
        }
    }
}
